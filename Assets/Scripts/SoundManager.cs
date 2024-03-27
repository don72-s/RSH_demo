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


    public void playbgm() {


        nDisplayer.ClearDisplayedNotes();
        nDisplayer.StopAllCoroutines();

        StopAllCoroutines();

        bgmplayer.Stop();

        StartCoroutine(playBGMReader());

    }

    public List<GameObject> noteButtons;


    public string LoadFileName;
    public void btn_androidLoadCheck() {

        StartCoroutine(loaddd());
    }


    IEnumerator loaddd() {

        // "StreamingAssets" 폴더에 있는 파일의 경로
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "stage1Note.dat");

        // 파일을 UnityWebRequest를 사용하여 로드
        UnityWebRequest www = UnityWebRequest.Get(streamingAssetsPath);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
             t.text = "Failed to load file: ";
        }
        else
        {
            // UnityWebRequest를 통해 로드한 파일의 바이트 데이터
            byte[] fileBytes = www.downloadHandler.data;

            // 파일을 "Application.persistentDataPath"로 저장
            string persistentDataPath = Path.Combine(Application.persistentDataPath, "stage1Note.dat");
            File.WriteAllBytes(persistentDataPath, fileBytes);

            t.text = "File copied to persistent data path:"+ persistentDataPath;
        }
    }

    private void Start()
    {
        LoadNodeData();
    }

    public void LoadNodeData() {

#if UNITY_EDITOR
        stageData = NoteDataManager.LoadData(PlayerPrefs.GetString("StageFileName"));
        //stageData = NoteDataManager.LoadData("stage1Note.dat");
#elif UNITY_ANDROID
        stageData = NoteDataManager.AndroidLoadData(PlayerPrefs.GetString("StageFileName"));
#endif

        if (stageData == null) {

            Debug.LogWarning("파일을 읽어올 수 없음.");
            return;

        }

        stageNoteArr = stageData.noteArray;

        bpmUnit = (float)(60 / ((double)stageData.bpm * stageData.bpmMultiplier));

        bgmplayer.clip = audioDic.GetBGMClip(stageData.bgmType);

        upperClip = audioDic.GetSEClip(stageData.upperSeType);
        lowerClip = audioDic.GetSEClip(stageData.lowerSeType);

        inputm.initSeClip(upperClip, lowerClip);

        Debug.Log("loaded");
    }

    public GameObject d;

    public InputField editPageField;

    public InputField inf;

    float bpmUnit = 0.1339285714285714f;

    private IEnumerator playBGMReader() {

        float countDelay = bpmUnit * stageData.bpmMultiplier;
        float playTime;

        if (bgmPause) Pause();

        bgmplayer.time = 0;
        
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

                    if (cnt == 3) break;

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
            yield return new WaitForSeconds(countDelay);
            bgmplayer.PlayOneShot(lowerClip);
            yield return new WaitForSeconds(countDelay);
            bgmplayer.PlayOneShot(lowerClip);
            yield return new WaitForSeconds(countDelay);
            bgmplayer.PlayOneShot(lowerClip);
            yield return new WaitForSeconds(countDelay);

        }
        


        //bgmplayer.Play();

        //todo : 이거 맵 정보로 바꾸기. ( 0.95f [ 수정 전 ] )
        //yield return new WaitForSeconds(stageData.offsetSecond);

        int bpmIndexer = 0;
        float bpmStacker = 0;
        int displayeIndexer = 0;
        float curBpmComparer = 0;

        int curEffectTimeUnit = 0;

        while (bpmStacker < 150) {// todo : 150 = 노래 길이로 수정 or 내부 while문의 두번째 조건으로 수정.

            //t.text = " " + bpmIndexer;

            while (curBpmComparer < bpmStacker && bpmIndexer < stageNoteArr.Length) {

                curBpmComparer += bpmUnit;



                if (stageNoteArr[bpmIndexer].waitScoreCount != 0) {

                    if(displayeIndexer % 2 == 0) nDisplayer.ClearDisplayedNotes();

                    curEffectTimeUnit = stageNoteArr[bpmIndexer].waitScoreCount;
                    nDisplayer.StartMovingMethod(stageNoteArr[bpmIndexer].waitScoreCount * bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit);

                    displayeIndexer++;

                }


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
        float waitTime = bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2);

        while (curTime < waitTime) {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        //판정라인
        //Debug.Log("ppp");
        //inputm.PlayDown();
        //inputm.PlayDown();

        curTime = 0;

        while (curTime < bpmUnit * 4) {

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

        //inputm.PlayUp();

        t.text = " failied ";



    }

    IEnumerator UpperNote(int _watingUnit)
    {

        inputm.PlayUp();
        nDisplayer.DisplayUpperNote();

        float curTime = 0;
        float waitTime = bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnit * 4)
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

        //inputm.PlayUp();
        t.text = " failied ";

    }

    IEnumerator InverseLowewrNote(int _watingUnit)
    {

        inputm.PlayDown();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseLowerNote();

        float curTime = 0;
        float waitTime = bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnit * 4)
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

        //inputm.PlayUp();

    }

    IEnumerator InverseUpperNote(int _watingUnit)
    {

        inputm.PlayUp();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseUpperNote();

        float curTime = 0;
        float waitTime = bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2);

        while (curTime < waitTime)
        {

            yield return null;
            if (!bgmPause) curTime += Time.deltaTime;

        }

        curTime = 0;

        while (curTime < bpmUnit * 4)
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

        //inputm.PlayDown();

    }


}
