using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour {

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

    [SerializeField]
    AlertWindow alertWindow;

    private void Awake() {

        Input.gyro.enabled = true;

    }

    /// <summary>
    /// 유저세팅(감도) 적용. 파일이 없었을경우 -4,4로 세팅된 파일을 다운로드해옴.
    /// </summary>
    public void Init() {

#if UNITY_EDITOR

        if (!NoteDataManager.CheckFileExist("userData.sav")) {
            NoteDataManager.SaveData(new UserData(-4, 4), "userData.sav");
        }

        userData = NoteDataManager.LoadUserData();
        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        DisplayUserSetting();

#elif UNITY_ANDROID

        userData = NoteDataManager.AndroidLoadUserData();

        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        DisplayUserSetting();

#endif

    }


    private void Update() {

        Vector3 vec = Input.gyro.rotationRate;
        sensitivityText.text = vec.z.ToString("F4");

#if UNITY_EDITOR

        if (userData == null) { sensitivityText.text = "Loading..."; return; }

#elif UNITY_ANDROID



        if (!isLowerPlaying && vec.z > userData.lowerOffset) {

            StartCoroutine(playLowerSnd());

        }

        if (!isUpperPlaying && vec.z < userData.upperOffset) {

            StartCoroutine(playUpperSnd());

        }

#endif

    }

    #region 소리 재생 코루틴

    bool isLowerPlaying = false;
    WaitForSeconds lowerDelay = new WaitForSeconds(0.125f);
    private IEnumerator playLowerSnd() {

        if (!isLowerPlaying) {

            audioPlayer.PlayOneShot(lowerClip);
            isLowerPlaying = true;

            yield return lowerDelay;

            isLowerPlaying = false;

        }

    }


    bool isUpperPlaying = false;
    WaitForSeconds upperDelay = new WaitForSeconds(0.125f);
    private IEnumerator playUpperSnd() {

        if (!isUpperPlaying) {

            audioPlayer.PlayOneShot(upperClip);
            isUpperPlaying = true;

            yield return upperDelay;

            isUpperPlaying = false;

        }

    }

    #endregion

    /// <summary>
    /// 현재 세팅 출력 갱신
    /// </summary>
    void DisplayUserSetting() {

        curSensInfoText.text = "upper : " + userData.upperOffset + " / lower : " + userData.lowerOffset;

    }


    //외부 버튼 콜백 함수
    public void Btn_SetUpperOffset(InputField _upperInputField) {

        if (!InputChecker.IsPositiveFloat(_upperInputField, true)) {

            userData.upperOffset = InputChecker.GetFloat(_upperInputField);
            PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
#if UNITY_EDITOR
            NoteDataManager.SaveData(userData, "userData.sav");
#elif UNITY_ANDROID
            NoteDataManager.AndroidSaveData(userData, "userData.sav");
#endif
            DisplayUserSetting();

        } else {

            alertWindow.ShowSingleAlertWindow("Upper값은 음수여야 합니다.");

        }


    }
    public void Btn_SetLowerOffset(InputField _lowerInputField) {

        if (InputChecker.IsPositiveFloat(_lowerInputField)) {

            userData.lowerOffset = InputChecker.GetFloat(_lowerInputField);
            PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);
#if UNITY_EDITOR
            NoteDataManager.SaveData(userData, "userData.sav");
#elif UNITY_ANDROID
            NoteDataManager.AndroidSaveData(userData, "userData.sav");
#endif
            DisplayUserSetting();

        } else {

            alertWindow.ShowSingleAlertWindow("Lower값은 양수여야 합니다.");

        }

    }
    public void Btn_InitOffset() {

        userData.upperOffset = -4;
        userData.lowerOffset = 4;
        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

#if UNITY_EDITOR
        NoteDataManager.SaveData(userData, "userData.sav");
#elif UNITY_ANDROID
        NoteDataManager.AndroidSaveData(userData, "userData.sav");
#endif
        DisplayUserSetting();

    }

}
