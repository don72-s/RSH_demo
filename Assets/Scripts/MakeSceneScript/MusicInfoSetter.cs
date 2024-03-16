using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MusicInfoSetter : MonoBehaviour
{

    AudioSource audioPlayer;

    [SerializeField]
    ScoreIOSystem BoarderSystem;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }


    public InputField sectionNumInputter;
    public InputField sectionLengthInputter;

    public Text debugText;


    public float offsetSecond = 0;  //싱크를 위한 앞부분 대기 시간.
    public float BPM;               //곡의 bpm [ 노래 자체 고유값, 임의 변경은 거의 불가능 ]
    public int BPM_Multiplyer;      //한마디의 큰 비트는 4개[기본], 각 큰 비트 사이를 몇개의 세분 비트로 쪼갤지 결정하는 입력변수.

    public int beats_In_A_Score;

    double unitBPMSecond;

    public void btn_TestPlay() { 
    
        StopAllCoroutines();

        InitNoteSheet();

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer) ;
        
        StartCoroutine(playBeatTest(offsetSecond, unitBPMSecond, BPM_Multiplyer));

    }

    public void btn_BGM_Pause() {

        if (audioPlayer.isPlaying)
        {

            audioPlayer.Pause();

        }
        else { 
            audioPlayer.UnPause();
        }

    }

    public void btn_BGM_SectionPlay() {

        StopAllCoroutines();

        if (LoadedSectionList == null) return;

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        float startTime = (float) (int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4 * unitBPMSecond);
        float duringTime = (float)(int.Parse(sectionLengthInputter.text) * BPM_Multiplyer * 8 * unitBPMSecond);


        debugText.text = "curSection : " + int.Parse(sectionNumInputter.text) + " ~ " + (int.Parse(sectionNumInputter.text) + (int.Parse(sectionLengthInputter.text) * 2) - 1);

        int startIdx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4;

        StartCoroutine(playSection(startIdx, offsetSecond, startTime, startTime + duringTime, unitBPMSecond));
    }


    private List<ButtonScript> LoadedSectionList = null;

    public void btn_Load_Section()
    {

        StopAllCoroutines();

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        int startIdx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4 ;
        int endIdx = startIdx + int.Parse(sectionLengthInputter.text) * BPM_Multiplyer * 4;


        debugText.text = "LoadedSection : " + int.Parse(sectionNumInputter.text) + " ~ " + (int.Parse(sectionNumInputter.text) + (int.Parse(sectionLengthInputter.text) * 2) - 1);

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        Array.Copy(noteArray, startIdx, sectionArr, 0, endIdx - startIdx);

        LoadedSectionList = BoarderSystem.SetNotes(sectionArr, BPM_Multiplyer);

        ClearBoarderImg();

    }



    public void btn_Save_Section() {

        if (LoadedSectionList == null) return;

        //todo : duringTime 설정해야 함. => 기본상태 채보 다음으로 순위 미룸.

        int startIdx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4;


        int halfCounter = LoadedSectionList.Count / 2;


        for (int i = 0; i < LoadedSectionList.Count / 2; i++) {

            NoteType type = LoadedSectionList[i].GetNoteType();

            noteArray[startIdx].noteType = type;

            type = type == NoteType.INVERSE_UPPER_NOTE ? NoteType.DOWN_NOTE : type;
            type = type == NoteType.INVERSE_DOWN_NOTE ? NoteType.UPPER_NOTE : type;

            noteArray[startIdx + halfCounter].noteType = type;

            startIdx++;

        }

    }


    public void ClearBoarderImg() {

        foreach (ButtonScript _btnScr in LoadedSectionList)
        {
            _btnScr.SetBoarderImg(false);
        }

    }



    private IEnumerator playSection(int _startIdx, float _offset, float _startTime,float _endTime, double _unitBPMSecond)
    {

        ClearBoarderImg();

        int noteCnt = _startIdx;
        int btnCnt = 0;

        double cmpTime = _startTime + _offset;
        double endTime = _endTime + _offset;

        audioPlayer.clip = bgmClip;
        audioPlayer.time = _startTime;
        audioPlayer.Play();


        while (audioPlayer.isPlaying && audioPlayer.time < endTime)
        {

            if (audioPlayer.time > cmpTime)
            {
                cmpTime += _unitBPMSecond;

                StartCoroutine(PlayNote(noteArray[noteCnt]));

                if (btnCnt < LoadedSectionList.Count) LoadedSectionList[btnCnt].SetBoarderImg(true);

                noteCnt++;
                btnCnt++;

            }

            yield return null;
        }

        audioPlayer.Stop();

    }


    private IEnumerator playBeatTest(float _offset, double _unitBPMSecond, int _bpmMultiCount) {

        double cmpTime = _offset;
        int cnt = 0;

        audioPlayer.clip = bgmClip;

        float audioLength = audioPlayer.clip.length;

        audioPlayer.Play();

        yield return new WaitForSeconds(_offset);

        while (audioPlayer.isPlaying || audioPlayer.time < audioLength) {

            if (audioPlayer.time > cmpTime)
            {
                cmpTime += _unitBPMSecond;

                //if (cnt % _bpmMultiCount == 0) audioPlayer.PlayOneShot(downFXClip, 1f);

                StartCoroutine(PlayNote(noteArray[cnt]));

                //if isEndSection => arrow reset? or just tile display?=>이쪽이 더 나아보이기도 함.

                debugText.text = "curCnt : " + cnt + " / arrLength : " + noteArray.Length;


                cnt++;

            }

            yield return null;
        }

    }



    public enum NoteType { NONE, DOWN_NOTE, UPPER_NOTE, INVERSE_DOWN_NOTE, INVERSE_UPPER_NOTE};
    private IEnumerator PlayNote(NoteInfo _noteInfo) {


        switch (_noteInfo.noteType) {

            case NoteType.DOWN_NOTE:
                audioPlayer.PlayOneShot(downFXClip);
                StartCoroutine(WaitInput(downFXClip, _noteInfo.delayTimeUnit));
                break;

            case NoteType.UPPER_NOTE:
                audioPlayer.PlayOneShot(upperFXClip);
                StartCoroutine(WaitInput(upperFXClip, _noteInfo.delayTimeUnit));
                break;

            case NoteType.INVERSE_DOWN_NOTE:
                audioPlayer.PlayOneShot(downFXClip);
                StartCoroutine(WaitInput(upperFXClip, _noteInfo.delayTimeUnit));
                break;

            case NoteType.INVERSE_UPPER_NOTE:
                audioPlayer.PlayOneShot(upperFXClip);
                StartCoroutine(WaitInput(downFXClip, _noteInfo.delayTimeUnit));
                break;

            default:
                yield return null;
                break;

        }

    }

    //todo : 판정 확인점으로 바꿔야 하는 함수, playoneshot을 이용하면 pause가 풀림. 이걸 우회할 방법 고려.!!
    //todo : 채보시에는 필요 없음!!!
    private IEnumerator WaitInput(AudioClip _snd, int _waitUnitTime) {

        //yield return new WaitForSeconds((float)(unitBPMSecond * BPM_Multiplyer * _waitUnitTime));//다음마디는 기본적으로 8
        //audioPlayer.PlayOneShot(_snd);
        yield return null;

    }



    internal struct NoteInfo {

        public NoteType noteType;
        public int delayTimeUnit;
        public int duringUnit;

    }

    private NoteInfo[] noteArray = null;

    public void InitNoteSheet() {

        if (bgmClip == null) return;

        float bgmTime = bgmClip.length;
        Debug.Log(bgmTime);
        float minOfLength = bgmTime / 60f;//1.666666666min

        int beatInBGM = (int)Mathf.Ceil(minOfLength * (int)BPM * BPM_Multiplyer);//노랫속 beat의 총 갯수

        int arrCount = beatInBGM + BPM_Multiplyer;


        //todo : 나머지 정리
        if (noteArray == null)
        {

            noteArray = new NoteInfo[arrCount];



            #region 채보부분에서 할 작업.


            int delayMultiplyer = int.Parse(sectionLengthInputter.text) * 4;


            noteArray[0].noteType = NoteType.DOWN_NOTE;
            noteArray[0].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer].noteType = NoteType.DOWN_NOTE;
            noteArray[BPM_Multiplyer].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer * 2].noteType = NoteType.INVERSE_DOWN_NOTE;
            noteArray[BPM_Multiplyer * 2].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer * 3].noteType = NoteType.INVERSE_DOWN_NOTE;//노트종류
            noteArray[BPM_Multiplyer * 3].delayTimeUnit = delayMultiplyer;//판정이 얼마나 뒤에 실행될지 결정하는 딜레이


            noteArray[BPM_Multiplyer * 4].noteType = NoteType.INVERSE_UPPER_NOTE;
            noteArray[BPM_Multiplyer * 4].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer * 5].noteType = NoteType.INVERSE_UPPER_NOTE;
            noteArray[BPM_Multiplyer * 5].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer * 6].noteType = NoteType.INVERSE_UPPER_NOTE;
            noteArray[BPM_Multiplyer * 6].delayTimeUnit = delayMultiplyer;


            noteArray[BPM_Multiplyer * 7].noteType = NoteType.INVERSE_UPPER_NOTE;//노트종류
            noteArray[BPM_Multiplyer * 7].delayTimeUnit = delayMultiplyer;//판정이 얼마나 뒤에 실행될지 결정하는 딜레이


            #endregion

        }



    }

    public void ClearNoteSheet() {
        noteArray = null;
    }



    public AudioClip bgmClip;
    public AudioClip downFXClip;
    public AudioClip upperFXClip;

}
