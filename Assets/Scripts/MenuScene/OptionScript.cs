using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour
{

    public UserData userData = null;

    [SerializeField]
    Text sensitivityText;

    [SerializeField]
    Text curSensInfoText;

    [SerializeField]
    AudioSource audioPlayer;

    [SerializeField]
    AudioClip upperClip;
    [SerializeField]
    AudioClip lowerClip;

    public void init()
    {
        Input.gyro.enabled = true;

#if UNITY_EDITOR

        if (!NoteDataManager.CheckFileExist("userData.sav")) {
            NoteDataManager.SaveData(new UserData(-4, 4), "userData.sav");
        }

        userData = NoteDataManager.LoadUserData();

        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        refreshSensInfo();


#elif UNITY_ANDROID

        userData = NoteDataManager.AndroidLoadUserData();
        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        refreshSensInfo();

#endif

    }

    private void Update()
    {

        Vector3 vec = Input.gyro.rotationRate;

        sensitivityText.text = vec.z.ToString();

#if UNITY_EDITOR

        if(userData == null) { sensitivityText.text = "Loading..."; return; }

#elif UNITY_ANDROID

        

        if (!isLowerPlaying && vec.z > userData.lowerOffset)
        {

            StartCoroutine(playLowerSnd());

        }

        if (!isUpperPlaying && vec.z < userData.upperOffset)
        {
            StartCoroutine(playUpperSnd());

        }

#endif

    }

    #region 소리 재생 코루틴

    private bool isLowerPlaying = false;
    private float lowerWaitTime = 0.25f;
    private IEnumerator playLowerSnd()
    {

        if (!isLowerPlaying)
        {
            audioPlayer.PlayOneShot(lowerClip);
            isLowerPlaying = true;

            yield return new WaitForSeconds(lowerWaitTime / 2);

            isLowerPlaying = false;

        }

    }


    private bool isUpperPlaying = false;
    private float upperWaitTime = 0.25f;
    private IEnumerator playUpperSnd()
    {

        if (!isUpperPlaying)
        {
            audioPlayer.PlayOneShot(upperClip);
            isUpperPlaying = true;

            yield return new WaitForSeconds(upperWaitTime / 2);

            isUpperPlaying = false;


        }

    }

    #endregion


    private void refreshSensInfo() {

        curSensInfoText.text = "upper : " + userData.upperOffset + " / lower : " + userData.lowerOffset;

    }



    public void btn_windowActive(bool _isActive) { 
        gameObject.SetActive(_isActive);
    }
    public void btn_SetUpperOffset(InputField _upperInputField) {

        float upperVal;

        if (float.TryParse(_upperInputField.text, out upperVal)) {
            userData.upperOffset = upperVal;
            PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
#if UNITY_EDITOR
            NoteDataManager.SaveData(userData, "userData.sav");
#elif UNITY_ANDROID
            NoteDataManager.AndroidSaveData(userData, "userData.sav");
#endif

            refreshSensInfo();
        }
        else
        {
            //alertWindow
        }

    }
    public void btn_SetLowerOffset(InputField _lowerInputField) {

        float lowerVal;

        if (float.TryParse(_lowerInputField.text, out lowerVal))
        {
            userData.lowerOffset = lowerVal;
            PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);
#if UNITY_EDITOR
            NoteDataManager.SaveData(userData, "userData.sav");
#elif UNITY_ANDROID
            NoteDataManager.AndroidSaveData(userData, "userData.sav");
#endif
            refreshSensInfo();
        }
        else
        {
            //alertWindow
        }

    }




    IEnumerator UnpackingNoteFile(string _fileName)
    {

        debug.text = "입장2";

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

        debug.text = "완료";


    }

    public Text debug;





}
