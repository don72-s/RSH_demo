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

/*    public void setArrayPart() {

        int baseIdx = int.Parse(editPageField.text) * 32;

        for (int i = 0; i < 16; i++)
        {

            noteArr[baseIdx + i] = noteButtons[i].GetComponent<noteScr>().myType;

        }

    }*/

    public string LoadFileName;


    public void btn_androidLoadCheck() {

        StartCoroutine(loaddd());
    }

    public void btn_androidLoadCheck222()
    {

        StartCoroutine(loaddd2());
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

    IEnumerator loaddd2()
    {

        // "StreamingAssets" 폴더에 있는 파일의 경로
        string streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets/stage1Note.dat";

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

            t.text = "File copied to persistent data path:";
        }
    }

    public void LoadNodeData() {

        //stageData = NoteDataManager.LoadData(LoadFileName);
        stageData = NoteDataManager.AndroidLoadData("stage1Note.dat");

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

/*    public void androidMapLoad() {
        t.text = Application.persistentDataPath;
        noteArr = NoteDataManager.AndroidMapLoadData();
    }*/

    public GameObject d;

    public InputField editPageField;



    public InputField inf;


    //const float bpmUnit = 0.53571428f; - base BPM
    float bpmUnit = 0.1339285714285714f;

    private IEnumerator playBGMReader() {

        yield return new WaitForSeconds(stageData.offsetSecond);

        int bpmIndexer = 0;
        float bpmStacker = 0;
        int displayeIndexer = 0;
        float curBpmComparer = 0;


        while (bpmStacker < 150) {

            t.text = " " + bpmIndexer;

            while (curBpmComparer < bpmStacker) {

                curBpmComparer += bpmUnit;


                if (stageNoteArr[bpmIndexer].effectTimeUnit != 0) {

                    if(displayeIndexer % 2 == 0) nDisplayer.ClearDisplayedNotes();

                    nDisplayer.StartMovingMethod(stageNoteArr[bpmIndexer].effectTimeUnit * bpmUnit * stageData.bpmMultiplier * stageData.scoreUnit);

                    displayeIndexer++;

                }


                switch (stageNoteArr[bpmIndexer].noteType)
                {

                    case NoteType.NONE:
                        break;

                    case NoteType.DOWN_NOTE:
                        StartCoroutine(LowewrNote());
                        break;

                    case NoteType.UPPER_NOTE:
                        StartCoroutine(UpperNote());
                        break;

                    case NoteType.INVERSE_DOWN_NOTE:
                        StartCoroutine(InverseLowewrNote());
                        break;

                    case NoteType.INVERSE_UPPER_NOTE:
                        StartCoroutine(InverseUpperNote());
                        break;



                }

                bpmIndexer++;
                

            }


            bpmStacker += Time.deltaTime;

            yield return null;

        }


    }

    IEnumerator LowewrNote() {

        inputm.PlayDown();
        nDisplayer.DisplayLowerNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayDown();

    }

    IEnumerator UpperNote()
    {

        inputm.PlayUp();
        nDisplayer.DisplayUpperNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayUp();

    }


    IEnumerator InverseLowewrNote()
    {

        inputm.PlayDown();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseLowerNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayDown();

    }

    IEnumerator InverseUpperNote()
    {

        inputm.PlayUp();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseUpperNote ();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayUp();

    }


}
