using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public AudioSource bgmplayer;

    public InputManager inputm;
    public NoteDisplayere nDisplayer;

    public Text t;

    [SerializeField]
    AudioDictonary audioDic;

    private AudioClip upperClip;
    private AudioClip lowerClip;

    StageInfo stageData;

    private NoteInfo[] stageNoteArr;


    private bool bgmPause = false;

    public void Pause()
    {

        if (!bgmPause)
        {
            bgmPause = true;
            bgmplayer.Pause();
        }
        else {
            bgmPause = false;
            bgmplayer.Play();
        }

        nDisplayer.setPause(bgmPause);

    }


    /// <summary>
    /// 스테이지를 시작시키는 함수.
    /// </summary>
    public void StartStage() {

        if (stageData == null)
        {
            Debug.LogWarning("스테이지가 제대로 로드되지 않음.");
            return;
        }

        nDisplayer.ClearDisplayedNotes();
        nDisplayer.StopAllCoroutines();

        StopAllCoroutines();

        bgmplayer.Stop();

        StartCoroutine(playBGMReader());

    }


    private void Start()
    {
        LoadNodeData();
    }



    //에디터 디버그용 불러오는 파일 이름.
    [SerializeField]
    string DebugLoadFileName;



    float bpmUnitSecond;

    public void LoadNodeData() {

#if UNITY_EDITOR
        //todo : 디버그용임.
        //PlayerPrefs.SetString("StageFileName", DebugLoadFileName);
        stageData = NoteDataManager.LoadData(PlayerPrefs.GetString("StageFileName"));
#elif UNITY_ANDROID
        stageData = NoteDataManager.AndroidLoadData(PlayerPrefs.GetString("StageFileName"));
#endif

        if (stageData == null) {

            Debug.LogWarning("파일을 읽어올 수 없음.");
            return;

        }

        stageNoteArr = stageData.noteArray;

        bpmUnitSecond = (float)(60 / ((double)stageData.bpm * stageData.bpmMultiplier));

        bgmplayer.clip = audioDic.GetBGMClip(stageData.bgmType);

        upperClip = audioDic.GetSEClip(stageData.upperSeType);
        lowerClip = audioDic.GetSEClip(stageData.lowerSeType);

        inputm.initSeClip(upperClip, lowerClip);

        Debug.Log("loaded");
    }



    /// <summary>
    /// 실질적인 플레이 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator playBGMReader() {

        float countDelay = bpmUnitSecond * stageData.bpmMultiplier;
        float playTime;

        if (bgmPause) Pause();

        bgmplayer.time = 0;


        //시작 카운트 다운 구현.

        t.text = "3";

        //카운트 도중 재생
        if (countDelay * 4 - stageData.offsetSecond > 0)
        {
            playTime = countDelay * 4 - stageData.offsetSecond;

            int cnt = 0;
            bool isBgmPlayed = false;

            bgmplayer.PlayOneShot(lowerClip);

            float curTime = 0;
            float cmpTime = countDelay;

            while (cnt < 4) {

                if (curTime > cmpTime) {

                    if (cnt == 3) { 
                        break; 
                    }

                    t.text = cnt == 2 ? "Go!" : (2 - cnt).ToString();

                    bgmplayer.PlayOneShot(lowerClip);

                    cmpTime += countDelay;
                    cnt++;
                }

                if (!isBgmPlayed && curTime > playTime) {
                    bgmplayer.Play();
                    isBgmPlayed = true;
                }

                curTime += Time.deltaTime;
                yield return null;

            }

        }
        else {//카운트에 들어가기 전부터 재생 => 테스트 안됨.
            playTime = stageData.offsetSecond - countDelay * 4;

            bgmplayer.Play();

            yield return new WaitForSeconds(playTime);

            bgmplayer.PlayOneShot(lowerClip);
            t.text = "3";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            t.text = "2";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            t.text = "1";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            t.text = "Go!";
            yield return new WaitForSeconds(countDelay);

        }
        

        int bpmIndexer = 0;
        float bpmStacker = 0;
        int displayeIndexer = 0;
        float curBpmComparer = 0;

        int curEffectTimeUnit = 0;

        while (bpmStacker < bgmplayer.clip.length + 5) {//노래 길이 + 5초


            while (curBpmComparer < bpmStacker && bpmIndexer < stageNoteArr.Length) {

                curBpmComparer += bpmUnitSecond;

                //마디가 넘어갈 때 할 행동.
                if (stageNoteArr[bpmIndexer].waitScoreCount != 0) {

                    if(displayeIndexer % 2 == 0) nDisplayer.ClearDisplayedNotes();

                    curEffectTimeUnit = stageNoteArr[bpmIndexer].waitScoreCount;
                    nDisplayer.StartMovingMethod(stageNoteArr[bpmIndexer].waitScoreCount * bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit);

                    displayeIndexer++;

                }


                //노트 분기
                switch (stageNoteArr[bpmIndexer].noteType)
                {

                    case NoteType.NONE:
                        break;

                    case NoteType.DOWN_NOTE:
                        StartCoroutine(LowewrNote(curEffectTimeUnit));
                        break;

                    case NoteType.UPPER_NOTE:
                        StartCoroutine(UpperNote(curEffectTimeUnit));
                        break;

                    case NoteType.INVERSE_DOWN_NOTE:
                        StartCoroutine(InverseLowewrNote(curEffectTimeUnit));
                        break;

                    case NoteType.INVERSE_UPPER_NOTE:
                        StartCoroutine(InverseUpperNote(curEffectTimeUnit));
                        break;



                }

                bpmIndexer++;
                

            }


            if(!bgmPause) bpmStacker += Time.deltaTime;

            yield return null;

        }


    }

    IEnumerator LowewrNote(int _watingUnit) {

        inputm.PlayDown();
        nDisplayer.DisplayLowerNote();

        float curTime = 0;
        float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

        while (curTime < waitTime) {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnitSecond * 4) {

            t.text = "start!";


            if (!bgmPause) curTime += Time.deltaTime;

            if (inputm.isLower()) {

                inputm.useLower();
                t.text = "correct";
                inputm.PlayDown();
                yield break;

            }

            yield return null;

        }

        t.text = " failied ";



    }

    IEnumerator UpperNote(int _watingUnit)
    {

        inputm.PlayUp();
        nDisplayer.DisplayUpperNote();

        float curTime = 0;
        float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnitSecond * 4)
        {

            t.text = "start!";


            if (!bgmPause) curTime += Time.deltaTime;

            if (inputm.isUpper())
            {

                inputm.useUpper();
                t.text = "correct";
                inputm.PlayUp();
                yield break;

            }

            yield return null;

        }

        t.text = " failied ";

    }

    IEnumerator InverseLowewrNote(int _watingUnit)
    {

        inputm.PlayDown();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseLowerNote();

        float curTime = 0;
        float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnitSecond * 4)
        {

            t.text = "start!";


            if (!bgmPause) curTime += Time.deltaTime;

            if (inputm.isUpper())
            {

                inputm.useUpper();
                t.text = "correct";
                inputm.PlayUp();
                yield break;

            }

            yield return null;

        }

        t.text = " failied ";

    }

    IEnumerator InverseUpperNote(int _watingUnit)
    {

        inputm.PlayUp();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseUpperNote();

        float curTime = 0;
        float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnitSecond * 4)
        {

            t.text = "start!";


            if (!bgmPause) curTime += Time.deltaTime;

            if (inputm.isLower())
            {

                inputm.useLower();
                t.text = "correct";
                inputm.PlayDown();
                yield break;

            }

            yield return null;

        }

        t.text = " failied ";

    }


}
