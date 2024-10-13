using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;

public class StageSceneLoader : MonoBehaviour {

    [Header("Default Stages Data")]
    [SerializeField]
    LoadFileNames fileInfoSO;

    [Header("Swipe Menu Obj")]
    [SerializeField]
    SwipeScript swipeMenuScr;

    [Header("Option Window Obj")]
    [SerializeField]
    OptionScript optionWindow;


    private void Start() {

        List<string> nameList = fileInfoSO.GetFileNames();


#if UNITY_EDITOR

        //에디터는 파일 확인 생략

        optionWindow.Init();
        swipeMenuScr.InitStageButtons(nameList);


        if (FileIOSystem.CheckHash(fileInfoSO)) {
            Debug.Log("해시 일치");
        } else {
            Debug.Log("불일치");
        }

#elif UNITY_ANDROID

        //sav파일이 없다면 새로 생성.
        if (!FileIOSystem.CheckFileExist("userData.sav"))
            StartCoroutine(AndroidUnpackingFile("userData.sav"));

        //sav파일 다운로드 완료까지 대기.
        StartCoroutine(AndroidUserdataDownloadCheck("userData.sav"));

        //dat파일 확인 (노트파일과 해시 무결성 체크)
        if (CheckDefaultFilesExist_Hash(fileInfoSO)) {

            //파일에 문제가 없는 경우 초기화하고 종료.
            InitElements(nameList);

        } else {

            //깨진 파일들이 감지되었으므로 파일들 제거
            RemoveFiles(fileInfoSO.GetFileNames());

            foreach (string _fileName in nameList) {

                StartCoroutine(AndroidUnpackingFile(_fileName));

            }

            StartCoroutine(AndroidNoteFilesDownloadCheck(fileInfoSO));

        }
#endif

    }


    /// <summary>
    /// 해당 파일들이 준비되어있는지와 해시 일치 확인.
    /// </summary>
    /// <param name="_fileInfoL">대상 파일 이름 리스트</param>
    /// <returns></returns>
    bool CheckDefaultFilesExist_Hash(LoadFileNames _fileNamesSO) {

        List<LoadFileNames.FileInfo> fileInfoL = _fileNamesSO.fileInfoL;

        foreach (LoadFileNames.FileInfo _fileInfo in fileInfoL) {//파일 존재 여부 확인.

            if (!File.Exists(Path.Combine(Application.persistentDataPath, _fileInfo.fileName))) {
                return false;
            }
        }

        foreach (LoadFileNames.FileInfo _fileInfo in fileInfoL) {//해시값 체크

            if (!FileIOSystem.CheckHash(_fileInfo.fileName, _fileInfo.sha256Hash)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 전달받은 이름의 모든 파일 제거(안드로이드).
    /// </summary>
    /// <param name="_fileNames">제거할 파일 이름 목록(확장자 포함)</param>
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


    /// <summary>
    /// 제공받은 기본 스테이지와 커스텀 스테이지를 읽어와 초기화
    /// </summary>
    /// <param name="_systemDefaultFileNames">기본 스테이지 파일의 이름들</param>
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


        //스테이지 버튼 세팅
        swipeMenuScr.InitStageButtons(fileNamesList);

    }




    /// <summary>
    /// 기기의 persistancepersistentDataPath에 streamingAssets폴더로부터 파일을 다운로드.
    /// </summary>
    /// <param name="_fileName">다운로드할 파일명(확장자 포함)</param>
    /// <returns></returns>
    IEnumerator AndroidUnpackingFile(string _fileName) {

        // "StreamingAssets" 폴더에 있는 파일의 경로
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, _fileName);

        

        // 파일을 UnityWebRequest를 사용하여 로드
        UnityWebRequest www = UnityWebRequest.Get(streamingAssetsPath);
        yield return www.SendWebRequest();

        try {

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log("네트워크 에러");
            } else {
                // UnityWebRequest를 통해 로드한 파일의 바이트 데이터
                byte[] fileBytes = www.downloadHandler.data;

                // 파일을 "Application.persistentDataPath"에 저장
                string persistentDataPath = Path.Combine(Application.persistentDataPath, _fileName);
                File.WriteAllBytes(persistentDataPath, fileBytes);

            }

        } catch (Exception _e) {
            Debug.Log(_e);
        } finally { 
            www.Dispose();
        }
    }

    /// <summary>
    /// 기기에 유저데이터 파일이 다운로드 되었는지 체크[15초]
    /// 다운로드가 완료된 경우에는 유저데이터 초기화.
    /// </summary>
    /// <param name="_fileName">확인할 파일명(확장자 포함)</param>
    IEnumerator AndroidUserdataDownloadCheck(string _fileName) {

        int downloadCount = 0;

        while (!FileIOSystem.CheckFileExist(_fileName)) {

            yield return new WaitForSeconds(3f);
            downloadCount++;

            if (downloadCount > 5) {
                yield break;
            }

        }

        optionWindow.Init();

    }


    /// <summary>
    /// 기기에 기본 제공 맵 파일들이 다운로드 완료되었는지 확인 [15초]
    /// 완료된 경우에는 초기화 진행.
    /// </summary>
    /// <param name="_fileInfoSO">다운로드 확인할 제공 스테이지 파일 데이터</param>
    IEnumerator AndroidNoteFilesDownloadCheck(LoadFileNames _fileInfoSO) {

        int downloadCount = 0;

        while (!CheckDefaultFilesExist_Hash(_fileInfoSO)) {

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
