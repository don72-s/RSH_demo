using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {

    AudioSource bgmplayer;

    [Header("Other Manager")]
    [SerializeField]
    InputManager inputm;
    [SerializeField]
    NoteDisplayere nDisplayer;

    [SerializeField]
    Text hitText;

    [Header("sfx DataSet")]
    [SerializeField]
    AudioDictonary audioDic;

    [SerializeField]
    Animator effectAnimator;
    [SerializeField]
    Animator failAnimator;
    [SerializeField]
    ScoreDisplayer scoreDisplayer;

    StageInfo stageData;
    NoteInfo[] stageNoteArr;


    AudioClip upperClip;
    AudioClip lowerClip;
    float bpmUnitSecond;


    private void Awake() {

        bgmplayer = GetComponent<AudioSource>();

    }

    private void Start() {

        LoadNodeData();
        scoreDisplayer.ResetBoard();

    }


    public void Pause() {

        if (Time.timeScale == 0) {

            Time.timeScale = 1;
            AudioListener.pause = false;

            bgmplayer.Play();

        } else {

            Time.timeScale = 0;
            AudioListener.pause = true;

            bgmplayer.Pause();

        }

    }


    /// <summary>
    /// 스테이지를 시작시키는 함수.
    /// </summary>
    public void GameStart() {

        if (stageData == null) {
            UnityEngine.Debug.LogWarning("스테이지가 제대로 로드되지 않음.");
            return;
        }

        nDisplayer.ClearDisplayedNotes();
        nDisplayer.StopAllCoroutines();

        StopAllCoroutines();

        bgmplayer.Stop();

        StartCoroutine(StageStartCO());

    }



    public void LoadNodeData() {

        stageData = FileIOSystem.LoadData(PlayerPrefs.GetString("StageFileName"));

        if (stageData == null) {

            UnityEngine.Debug.LogWarning("파일을 읽어올 수 없음.");
            return;

        }

        stageNoteArr = stageData.noteArray;

        bpmUnitSecond = (float)(60 / ((double)stageData.bpm * stageData.bpmMultiplier));

        bgmplayer.clip = audioDic.GetBGMClip(stageData.bgmType);

        upperClip = audioDic.GetSEClip(stageData.upperSeType);
        lowerClip = audioDic.GetSEClip(stageData.lowerSeType);

        inputm.InitSeClip(upperClip, lowerClip);

    }


    /// <summary>
    /// 실질적인 플레이 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator StageStartCO() {


        float countDelay = bpmUnitSecond * stageData.bpmMultiplier;
        float playTime;

        bgmplayer.time = 0;

        //시작 카운트 다운
        hitText.text = "3";

        //카운트 도중 재생
        if (countDelay * 4 - stageData.offsetSecond > 0) {
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

                    hitText.text = cnt == 2 ? "Go!" : (2 - cnt).ToString();

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

        } else {//카운트에 들어가기 전부터 재생

            playTime = stageData.offsetSecond - countDelay * 4;

            bgmplayer.Play();

            yield return new WaitForSeconds(playTime);

            bgmplayer.PlayOneShot(lowerClip);
            hitText.text = "3";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            hitText.text = "2";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            hitText.text = "1";
            yield return new WaitForSeconds(countDelay);

            bgmplayer.PlayOneShot(lowerClip);
            hitText.text = "Go!";
            yield return new WaitForSeconds(countDelay);

        }


        int bpmIndexer = 0;
        int displayeIndexer = 0;
        double curBpmComparer = 0;


        //여러가지 시도의 흔적
        double testBaseCurTime = AudioSettings.dspTime;

        int curEffectTimeUnit = 0;

        while (/*Stacking*/AudioSettings.dspTime - testBaseCurTime < bgmplayer.clip.length + 5) {//노래 길이 + 5초


            while (curBpmComparer < /*bpmStacker*/AudioSettings.dspTime - testBaseCurTime && bpmIndexer < stageNoteArr.Length) {

                curBpmComparer = bpmUnitSecond * (bpmIndexer + 1);

                //마디가 넘어갈 때 할 행동.
                if (stageNoteArr[bpmIndexer].waitScoreCount != 0) {

                    if (displayeIndexer % 2 == 0) nDisplayer.ClearDisplayedNotes();

                    curEffectTimeUnit = stageNoteArr[bpmIndexer].waitScoreCount;
                    nDisplayer.StartMovingMethod(stageNoteArr[bpmIndexer].waitScoreCount * bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit);

                    displayeIndexer++;

                }


                //노트 분기
                switch (stageNoteArr[bpmIndexer].noteType) {

                    case NoteType.NONE:
                        break;

                    case NoteType.DOWN_NOTE:
                        StartCoroutine(PlayNote(curEffectTimeUnit, inputm.PlayDown, nDisplayer.DisplayLowerNote, inputm.isLower, inputm.useLower, inputm.PlayDown));
                        break;

                    case NoteType.UPPER_NOTE:
                        StartCoroutine(PlayNote(curEffectTimeUnit, inputm.PlayUp, nDisplayer.DisplayUpperNote, inputm.isUpper, inputm.useUpper, inputm.PlayUp));
                        break;

                    case NoteType.INVERSE_DOWN_NOTE:
                        StartCoroutine(PlayNote(curEffectTimeUnit, inputm.PlayDown, nDisplayer.DisplayInverseLowerNote, inputm.isUpper, inputm.useUpper, inputm.PlayUp));
                        break;

                    case NoteType.INVERSE_UPPER_NOTE:
                        StartCoroutine(PlayNote(curEffectTimeUnit, inputm.PlayUp, nDisplayer.DisplayInverseUpperNote, inputm.isLower, inputm.useLower, inputm.PlayDown));
                        break;



                }

                bpmIndexer++;

            }
            
/*            bpmStacker += Time.deltaTime;
            UnityEngine.Debug.Log("========================================");
            UnityEngine.Debug.Log("deltaTime : " + bpmStacker);
            UnityEngine.Debug.Log("StopWatch : " + (testStopWatch.Elapsed.TotalSeconds - 1));
            UnityEngine.Debug.Log("AudioDsp : " + (AudioSettings.dspTime - testBaseCurTime));
            UnityEngine.Debug.Log("audioTime : " + (bgmplayer.time - 1));
            UnityEngine.Debug.Log("================ diffs =================");
            UnityEngine.Debug.Log("deltaTime Diff : " + (bpmStacker - (bgmplayer.time - 1)));
            UnityEngine.Debug.Log("StopWatch Diff : " + (testStopWatch.Elapsed.TotalSeconds - 1 - (bgmplayer.time - 1)));
            UnityEngine.Debug.Log("AudioDsp Diff : " + (AudioSettings.dspTime - testBaseCurTime - (bgmplayer.time - 1)) );*/

            yield return null;

        }


    }


    /// <summary>
    /// 개별 노트의 코루틴 플레이 진행
    /// </summary>
    /// <param name="_watingUnit">다음 마디까지의 대기 단위</param>
    /// <param name="_swipeSnd">디스플레이 시 플레이 될 사운드</param>
    /// <param name="_displayNote">디스플레이 노트 종류</param>
    /// <param name="_checkInput">올바른 입력 판정 확인 함수</param>
    /// <param name="_useInput">사용할 입력 종류(중복입력 방지용)</param>
    /// <param name="_playSnd">입력 성공시 플레이 될 사운드</param>
    /// <returns></returns>
    IEnumerator PlayNote(int _watingUnit, Action _swipeSnd, Action _displayNote, Func<bool> _checkInput, Action _useInput, Action _playSnd) {

        _swipeSnd?.Invoke();
        _displayNote?.Invoke();

        float curTime = 0;
        float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

        yield return new WaitForSeconds(waitTime);//판정영역까지 대기

        float duringTime = bpmUnitSecond * 4;
        float goodEndDuringTime = bpmUnitSecond * 3;

        while (curTime < duringTime) {
            curTime += Time.deltaTime;

            if (_checkInput())//노트에 대응하는 입력인 경우.
            {

                _useInput?.Invoke();//입력한 플래그를 사용
                _playSnd?.Invoke();//노트 히트 사운드 재생


                if (curTime > bpmUnitSecond && curTime < goodEndDuringTime) {
                    hitText.text = "correct";
                    effectAnimator.SetTrigger("Correct");
                    scoreDisplayer.AddCorrect();
                } else {
                    hitText.text = "good";
                    effectAnimator.SetTrigger("Good");
                    scoreDisplayer.AddGood();
                }

                yield break;

            }

            yield return null;

        }

        effectAnimator.SetTrigger("Fail");
        failAnimator.SetTrigger("Fail");
        Handheld.Vibrate();
        scoreDisplayer.AddFail();
        hitText.text = " failied ";
    }


}
