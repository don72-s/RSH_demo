using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StageSceneLoader : MonoBehaviour
{
    [SerializeField]
    List<string> stageFileNameList;

    [SerializeField]
    SwipeScript swipeMenuScr;

    [SerializeField]
    GameObject ButtonParentObject;

    [SerializeField]
    GameObject ButtonInstance;

    [SerializeField]
    Sprite addBoarderImg;



    private List<SwiptBtnScript> buttonList = new List<SwiptBtnScript>();


    private void Start()
    {

#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (checkDefaultFilesExist(stageFileNameList))
        {

            InitElements(stageFileNameList);
            return;

        }

        foreach (string _fileName in stageFileNameList)
        {

            StartCoroutine(UnpackingNoteFile(_fileName));

        }

        StartCoroutine(FileDownloadCheck(stageFileNameList));
#endif

    }

    public Text debugt;


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

        int totalMapCount = _systemDefaultFileNames.Count + customNoteFileNameL.Count;

        //오브젝트 풀 크기 체크
        while (buttonList.Count <= totalMapCount) {
            GameObject tmpBtnObj = Instantiate(ButtonInstance);
            tmpBtnObj.transform.SetParent(ButtonParentObject.transform);
            buttonList.Add(tmpBtnObj.GetComponent<SwiptBtnScript>());
        }

        //모두 비활성화
        foreach (SwiptBtnScript _btn in buttonList) {
            _btn.gameObject.SetActive(false);
        }

        //시스템 제공 스테이지 우선 활성화
        for (int i = 0; i < _systemDefaultFileNames.Count; i++) {
            buttonList[i].gameObject.SetActive(true);
            buttonList[i].SetText(_systemDefaultFileNames[i].Substring(0, _systemDefaultFileNames[i].Length - ".dat".Length));
        }


        //사용자 제작 스테이지 활성화
        for (int i = 0; i < customNoteFileNameL.Count; i++) {

            buttonList[i + _systemDefaultFileNames.Count].gameObject.SetActive(true);
            buttonList[i + _systemDefaultFileNames.Count].SetText(customNoteFileNameL[i].Substring(0, customNoteFileNameL[i].Length - ".dat".Length));

        }

        //추가 버튼 활성화.
        buttonList[totalMapCount].gameObject.SetActive(true);
        buttonList[totalMapCount].gameObject.GetComponent<Image>().sprite = addBoarderImg;
        buttonList[totalMapCount].SetText("+");


        //todo : 버튼 클릭시 이벤트 설정.

        //스와이프 재계산
        swipeMenuScr.Recalculate(totalMapCount + 1);

    }



    bool checkDefaultFilesExist(List<string> _fileNames) {

        foreach (string _fileName in _fileNames) {

            if (!File.Exists(Path.Combine(Application.persistentDataPath, _fileName))){
                return false;
            }
        }
        return true;
    }

    IEnumerator UnpackingNoteFile(string _fileName)
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

    IEnumerator FileDownloadCheck(List<string> _fileNames) {

        int downloadCount = 0;

        while (!checkDefaultFilesExist(_fileNames)) {

            yield return new WaitForSeconds(3f);
            downloadCount++;

            if (downloadCount > 5) {
                Debug.Log("다운로드 실패!");
                yield break;
            }

        }

        Debug.Log("다운로드 완료");
        InitElements(stageFileNameList);

    }

}
