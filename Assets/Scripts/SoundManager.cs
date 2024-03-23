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

    public void Pause()
    {
        StopAllCoroutines();
        nDisplayer.StopAllCoroutines();
        bgmplayer.Stop();
        nDisplayer.ClearDisplayedNotes();
    }


    public void playbgm() {


        nDisplayer.ClearDisplayedNotes();
        nDisplayer.StopAllCoroutines();

        StopAllCoroutines();

        bgmplayer.Stop();
        bgmplayer.Play();

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

   

    public void LoadNodeData() {

#if UNITY_EDITOR
        stageData = NoteDataManager.LoadData(LoadFileName);
#elif UNITY_ANDROID
        stageData = NoteDataManager.AndroidLoadData("stage1Note.dat");
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

        yield return new WaitForSeconds(0.95f);

        int bpmIndexer = 0;
        float bpmStacker = 0;
        int displayeIndexer = 0;
        float curBpmComparer = 0;

        int curEffectTimeUnit = 0;

        while (bpmStacker < 150) {// todo : 150 = 노래 길이로 수정

            //t.text = " " + bpmIndexer;

            while (curBpmComparer < bpmStacker) {

                curBpmComparer += bpmUnit;



                if (stageNoteArr[bpmIndexer].effectTimeUnit != 0) {

                    if(displayeIndexer % 2 == 0) nDisplayer.ClearDisplayedNotes();

                    curEffectTimeUnit = stageNoteArr[bpmIndexer].effectTimeUnit;
                    nDisplayer.StartMovingMethod(stageNoteArr[bpmIndexer].effectTimeUnit * bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit);

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


            bpmStacker += Time.deltaTime;

            yield return null;

        }


    }

    IEnumerator LowewrNote(int _watingUnit) {

        inputm.PlayDown();
        nDisplayer.DisplayLowerNote();

        yield return new WaitForSeconds(bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2 ));

        //판정라인
        //Debug.Log("ppp");
        //inputm.PlayDown();
        //inputm.PlayDown();

        float curTime = 0;

        while (curTime < bpmUnit * 4) {

            t.text = "start!";


            curTime += Time.deltaTime;

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

        yield return new WaitForSeconds(bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2));

        float curTime = 0;

        while (curTime < bpmUnit * 4)
        {

            t.text = "start!";


            curTime += Time.deltaTime;

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

        yield return new WaitForSeconds(bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2));

        float curTime = 0;

        while (curTime < bpmUnit * 4)
        {

            t.text = "start!";


            curTime += Time.deltaTime;

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

        yield return new WaitForSeconds(bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnit * 2));

        float curTime = 0;

        while (curTime < bpmUnit * 4)
        {

            t.text = "start!";


            curTime += Time.deltaTime;

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
