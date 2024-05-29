using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StageSceneLoader : MonoBehaviour
{
    
    [SerializeField]//scriptable object
    LoadFileNames fileInfoSO;

    [SerializeField]
    SwipeScript swipeMenuScr;

    [SerializeField]
    GameObject ButtonParentObject;

    [SerializeField]
    GameObject ButtonInstance;

    [SerializeField]
    Sprite addBoarderImg;

    [SerializeField]
    OptionScript optionWindow;


    //private List<SwiptBtnScript> buttonList = new List<SwiptBtnScript>();


    private void Start()
    {

        List<string> nameList = fileInfoSO.GetFileNames();

        optionWindow.init();

        swipeMenuScr.SettingSwipteButtons(nameList);

#if UNITY_EDITOR


        if (NoteDataManager.CheckHash(fileInfoSO)){
            Debug.Log("해시 일치");
        }else {
            Debug.Log("불일치");
        }

#elif UNITY_ANDROID

        //sav파일 확인
        if (!NoteDataManager.CheckAndroidFileExist("userData.sav"))
        {
            StartCoroutine(AndroidUnpackingNoteFile("userData.sav"));
        }

        StartCoroutine(AndroidUserdataDownloadCheck("userData.sav"));



        //dat파일 확인 (노트파일과 해시 무결성 체크)
        if (checkDefaultFilesExist(fileInfoSO))
        {

            InitElements(nameList);
            return;

        }

        //깨진 파일들이 감지되었으므로 파일들 제거
        RemoveFiles(fileInfoSO.GetFileNames());

        foreach (string _fileName in nameList)
        {

            StartCoroutine(AndroidUnpackingNoteFile(_fileName));

        }

        StartCoroutine(AndroidNoteFilesDownloadCheck(fileInfoSO));
#endif




    }

    public Text debugt;

    /// <summary>
    /// 해당 파일들이 준비되어있는지 확인
    /// </summary>
    /// <param name="_fileInfoL">대상 파일 이름 리스트</param>
    /// <returns></returns>
    bool checkDefaultFilesExist(LoadFileNames _fileNamesSO) {

        List<LoadFileNames.FileInfo> fileInfoL = _fileNamesSO.fileInfoL;

        foreach (LoadFileNames.FileInfo _fileInfo in fileInfoL) {//파일 존재 여부 확인.

            if (!File.Exists(Path.Combine(Application.persistentDataPath, _fileInfo.fileName))){
                return false;
            }
        }

        foreach (LoadFileNames.FileInfo _fileInfo in fileInfoL)
        {//해시값 체크

            if (!NoteDataManager.CheckHash(_fileInfo.fileName, _fileInfo.sha256Hash))
            {
                return false;
            }
        }

        return true;
    }

    void RemoveFiles(List<string> _fileNames) {

        foreach (string _deleteName in _fileNames)//일부만 존재시 전부 제거.
        {
            File.Delete(Path.Combine(Application.persistentDataPath, _deleteName));
        }

    }

    /// <summary>
    /// 제공 리스트에 제공되는 문자열 요소가 포함되는지 확인
    /// </summary>
    /// <param name="_compareGroup">확인할 리스트</param>
    /// <param name="_str">체크할 문자열 요소</param>
    /// <returns></returns>
    private bool CheckContainsString(List<string> _compareGroup, string _str) {

        foreach (string _string in _compareGroup) {

            if (_string.Equals(_str)) return true;

        }

        return false;

    }


    private void InitElements(List<string> _systemDefaultFileNames) {

        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        List<string> customNoteFileNameL = new List<string>();

        //파일형식 .dat 및 커스텀 맵 확인
        foreach (string _filePath in fileNames) {

            string fileName = _filePath.Substring(Application.persistentDataPath.Length + 1);

            if (_filePath.EndsWith(".dat") && !CheckContainsString(_systemDefaultFileNames, fileName)) { 
                customNoteFileNameL.Add(fileName);
            }

        }

        List<string> fileNamesList = new List<string>();
        fileNamesList.AddRange(_systemDefaultFileNames);
        fileNamesList.AddRange(customNoteFileNameL);


        //버튼 세팅
        swipeMenuScr.SettingSwipteButtons(fileNamesList);

    }





    IEnumerator AndroidUnpackingNoteFile(string _fileName)
    {

        // "StreamingAssets" 폴더에 있는 파일의 경로
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, _fileName);

        // 파일을 UnityWebRequest를 사용하여 로드
        UnityWebRequest www = UnityWebRequest.Get(streamingAssetsPath);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("네트워크 에러");
        }
        else
        {
            // UnityWebRequest를 통해 로드한 파일의 바이트 데이터
            byte[] fileBytes = www.downloadHandler.data;

            // 파일을 "Application.persistentDataPath"에 저장
            string persistentDataPath = Path.Combine(Application.persistentDataPath, _fileName);
            File.WriteAllBytes(persistentDataPath, fileBytes);

        }
    }

    IEnumerator AndroidUserdataDownloadCheck(string _fileName) {

        int downloadCount = 0;

        while (!NoteDataManager.CheckAndroidFileExist(_fileName))
        {

            yield return new WaitForSeconds(3f);
            downloadCount++;

            if (downloadCount > 5)
            {
                yield break;
            }

        }

        optionWindow.init();

    }

    IEnumerator AndroidNoteFilesDownloadCheck(LoadFileNames _fileInfoSO) {

        int downloadCount = 0;

        while (!checkDefaultFilesExist(_fileInfoSO)) {

            yield return new WaitForSeconds(3f);
            downloadCount++;

            if (downloadCount > 5) {
                Debug.Log("다운로드 실패!");
                yield break;
            }

        }

        Debug.Log("다운로드 완료");
        InitElements(_fileInfoSO.GetFileNames());

    }




}
