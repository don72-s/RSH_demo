using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip up;
    [SerializeField]
    private AudioClip down;

    [SerializeField]
    private AudioSource audioPlayer;

    [SerializeField]
    private Text text;

    [SerializeField]
    private float lowerOffset = 4;
    [SerializeField]
    private float upperOffset = 4;



    public InputField inputf;


    public void Start()
    {
        Input.gyro.enabled = true;
    }


    public void initSeClip(AudioClip _upperSnd, AudioClip _lowewrSnd) { 
    
        up = _upperSnd;
        down = _lowewrSnd;

        lowerFlag = false;
        upperFlag = false;
    }


    private void Update()
    {


        Vector3 vec = Input.gyro.rotationRate;

        //롱노트 반응 => 추후 기회되면 추가.
        /*        if (!isPlaying && vec.z > delayOffset && Input.touchCount >= 1) { 

                    PlayGazing();
                    StartCoroutine(playSnd(1.5f));

                }
        */

        #region 에디터용 디버그 

        if (!isLowerPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pushed");
            StartCoroutine(playLowerSnd());

        }


        if (!isUpperPlaying && Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("pushed");
            StartCoroutine(playUpperSnd());

        }

        #endregion

        if (!isLowerPlaying && vec.z > lowerOffset) {

            StartCoroutine(playLowerSnd());

        }

        if (!isUpperPlaying && vec.z < -upperOffset)
        {
            StartCoroutine(playUpperSnd());

        }

    }

    private bool lowerFlag = false;
    private bool upperFlag = false;

    public bool isUpper() {
        return upperFlag;
    }
    public void useUpper() {
        upperFlag = false;
    }

    public bool isLower()
    {
        return lowerFlag;
    }
    public void useLower()
    {
        lowerFlag = false;
    }


    
    private bool isLowerPlaying = false;
    private float lowerWaitTime = 0.25f;
    private IEnumerator playLowerSnd()
    {

        if (!isLowerPlaying)
        {

            isLowerPlaying = true;
            lowerFlag = true;

            yield return new WaitForSeconds(Time.deltaTime);

            lowerFlag = false;

            yield return new WaitForSeconds(lowerWaitTime / 2);

            isLowerPlaying = false;

        }

    }

    private bool isUpperPlaying = false;
    private float upperWaitTime = 0.25f;
    private IEnumerator playUpperSnd() {

        if (!isUpperPlaying) {

            isUpperPlaying = true;
            upperFlag = true;

            yield return new WaitForSeconds(Time.deltaTime);

            upperFlag = false;

            yield return new WaitForSeconds(upperWaitTime / 2);

            isUpperPlaying = false;


        }
    
    }


    /*private IEnumerator playSnd(float needDelta)
    {

        int breakCount = 10;
        int curCount = 0;

        if (!isUpperPlaying)
        {

            isUpperPlaying = !isUpperPlaying;


            while (curCount <= breakCount && Input.touchCount >= 1)
            {

                float zValue = Input.gyro.rotationRate.z;

                if (zValue < needDelta && zValue > -needDelta)
                {
                    curCount++;
                }
                else {
                    curCount = 0;
                }

                yield return null;

            }

            isUpperPlaying = false;
            StopGazing();

        }

    }*/



    public void PlayDown() {

        audioPlayer.PlayOneShot(down);

    }

    public void PlayUp() {

        audioPlayer.PlayOneShot(up);

    }

/*    public void PlayGazing() {
        audioPlayer.clip = down;
        audioPlayer.loop = true;
        audioPlayer.Play();
    }*/

/*    public void StopGazing() {
        audioPlayer.Stop();
        audioPlayer.loop = false;
    }*/

}
