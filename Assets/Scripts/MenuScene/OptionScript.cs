using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OptionScript : MonoBehaviour {

    public UserData userData = null;

    [SerializeField]
    TextMeshProUGUI curSensInfoText;

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

        if (!FileIOSystem.CheckFileExist("userData.sav")) {
            FileIOSystem.SaveData(new UserData(-4, 4), "userData.sav");
        }

#endif
        userData = FileIOSystem.LoadUserData();
        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        DisplayUserSetting();


    }



    private void Update() {

        float z = Input.gyro.rotationRate.z;

        if (!isLowerPlaying && z > userData.lowerOffset) {

            StartCoroutine(playLowerSnd());

        }

        if (!isUpperPlaying && z < userData.upperOffset) {

            StartCoroutine(playUpperSnd());

        }

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

        curSensInfoText.text = $"upper : {userData.upperOffset} \nlower : {userData.lowerOffset}";

    }


    //외부 버튼 콜백 함수
    public void Btn_SetUpperOffset(TMP_InputField _upperInputField) {

        if (InputChecker.IsNeagtiveFloat(_upperInputField)) {

            userData.upperOffset = InputChecker.GetFloat(_upperInputField);
            PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
            FileIOSystem.SaveData(userData, "userData.sav");
            DisplayUserSetting();

        } else {

            alertWindow.ShowSingleAlertWindow("UpperOffset must be a negative number.");

        }


    }
    public void Btn_SetLowerOffset(TMP_InputField _lowerInputField) {

        if (InputChecker.IsPositiveFloat(_lowerInputField)) {

            userData.lowerOffset = InputChecker.GetFloat(_lowerInputField);
            PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);
            FileIOSystem.SaveData(userData, "userData.sav");
            DisplayUserSetting();

        } else {

            alertWindow.ShowSingleAlertWindow("LowerOffset must be a positive number.");

        }

    }
    public void Btn_InitOffset() {

        userData.upperOffset = -4;
        userData.lowerOffset = 4;
        PlayerPrefs.SetFloat("UpperSensitivity", userData.upperOffset);
        PlayerPrefs.SetFloat("LowerSensitivity", userData.lowerOffset);

        FileIOSystem.SaveData(userData, "userData.sav");
        DisplayUserSetting();

    }

}
