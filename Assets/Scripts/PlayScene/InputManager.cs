using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InputManager : MonoBehaviour {

    [Header("Note's SFX")]
    [SerializeField]
    AudioClip upperSFX;
    [SerializeField]
    AudioClip lowerSFX;

    AudioSource audioPlayer;

    float lowerOffset;
    float upperOffset;

    bool lowerFlag = false;
    bool upperFlag = false;

    WaitForSeconds inputDelay = new WaitForSeconds(0.125f);


    private void Awake() {

        audioPlayer = GetComponent<AudioSource>();

    }

    private void Start() {

        Input.gyro.enabled = true;
        lowerOffset = PlayerPrefs.GetFloat("LowerSensitivity");
        upperOffset = PlayerPrefs.GetFloat("UpperSensitivity");

    }


    public void InitSeClip(AudioClip _upperSnd, AudioClip _lowewrSnd) {

        upperSFX = _upperSnd;
        lowerSFX = _lowewrSnd;

        lowerFlag = false;
        upperFlag = false;
    }


    private void Update() {


        Vector3 vec = Input.gyro.rotationRate;

        #region 에디터용 디버그 

        if (!isLowerPlaying && Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("pushed");
            StartCoroutine(playLowerSnd());

        }


        if (!isUpperPlaying && Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("pushed");
            StartCoroutine(playUpperSnd());

        }

        #endregion

        if (!isLowerPlaying && vec.z > lowerOffset) {

            StartCoroutine(playLowerSnd());

        }

        if (!isUpperPlaying && vec.z < upperOffset) {
            StartCoroutine(playUpperSnd());

        }

    }


    public bool isUpper() {
        return upperFlag;
    }
    public void useUpper() {
        upperFlag = false;
    }

    public bool isLower() {
        return lowerFlag;
    }
    public void useLower() {
        lowerFlag = false;
    }



    bool isLowerPlaying = false;
    private IEnumerator playLowerSnd() {

        if (!isLowerPlaying) {

            isLowerPlaying = true;
            lowerFlag = true;

            yield return null;

            lowerFlag = false;

            yield return inputDelay;

            isLowerPlaying = false;

        }

    }

    bool isUpperPlaying = false;
    private IEnumerator playUpperSnd() {

        if (!isUpperPlaying) {

            isUpperPlaying = true;
            upperFlag = true;

            yield return null;

            upperFlag = false;

            yield return inputDelay;

            isUpperPlaying = false;


        }

    }


    public void PlayDown() {

        audioPlayer.PlayOneShot(lowerSFX);

    }

    public void PlayUp() {

        audioPlayer.PlayOneShot(upperSFX);

    }

}
