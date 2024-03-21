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
    private float delayOffset = 4;


    public InputField inputf;


    public void Start()
    {
        Input.gyro.enabled = true;
    }

    public void setOffset() {

        delayOffset = float.Parse(inputf.text);

    }


    public void initSeClip(AudioClip _upperSnd, AudioClip _lowewrSnd) { 
    
        up = _upperSnd;
        down = _lowewrSnd;

    }


    private void Update()
    {


        Vector3 vec = Input.gyro.rotationRate;


        if (!isPlaying && vec.z > delayOffset && Input.touchCount >= 1) {

            PlayGazing();
            StartCoroutine(playSnd(1.5f));

        }

        if (!isDownPlaying && vec.z > delayOffset) {

            PlayDown();
            StartCoroutine(playDownSnd());

        }

        if (!isPlaying && vec.z < -delayOffset * 1.5f)
        {

            PlayUp();
            StartCoroutine(playSnd());

        }

    }


    private bool isPlaying = false;
    private float waitTime = 0.25f;
    private float curTime = 0;
    private IEnumerator playSnd() {

        if (!isPlaying) {

            isPlaying = !isPlaying;

            while (curTime < waitTime) {

                curTime += Time.deltaTime;
                yield return null;
            
            }

            curTime = 0;
            isPlaying = false;


        }
    
    }


    private bool isDownPlaying = false;
    private float DownwaitTime = 0.25f;
    private float DowncurTime = 0;
    private IEnumerator playDownSnd()
    {

        if (!isDownPlaying)
        {

            isDownPlaying = !isDownPlaying;

            while (DowncurTime < DownwaitTime)
            {

                DowncurTime += Time.deltaTime;
                yield return null;

            }

            DowncurTime = 0;
            isDownPlaying = false;


        }

    }


    private IEnumerator playSnd(float needDelta)
    {

        int breakCount = 10;
        int curCount = 0;

        if (!isPlaying)
        {

            isPlaying = !isPlaying;


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

            isPlaying = false;
            StopGazing();

        }

    }



    public void PlayDown() {

        audioPlayer.PlayOneShot(down);

    }

    public void PlayUp() {

        audioPlayer.PlayOneShot(up);

    }

    public void PlayGazing() {
        audioPlayer.clip = down;
        audioPlayer.loop = true;
        audioPlayer.Play();
    }

    public void StopGazing() {
        audioPlayer.Stop();
        audioPlayer.loop = false;
    }

}
