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

        //test
        //unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

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

        //todo : duringTime 설정해야 함. => 기본상태 채보 다음으로 순위 미룸.[차지하는 악보 마디 ㅇㅇ]

        int idx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4;

        for (int i = 0; i < LoadedSectionList.Count / 2; i++) {

            NoteType type = LoadedSectionList[i].GetNoteType();

            noteArray[idx].noteType = type;
            noteArray[idx].waitingUnit = int.Parse(sectionLengthInputter.text) * 4;
            noteArray[idx].effectTimeUnit = 0;

            idx++;

        }

        noteArray[int.Parse(sectionNumInputter.text) * BPM_Multiplyer * 4].effectTimeUnit = int.Parse(sectionLengthInputter.text);

        if (idx < noteArray.Length)
        {
            Debug.Log("idx : " + idx);
            noteArray[idx].effectTimeUnit = int.Parse(sectionLengthInputter.text);
            noteArray[idx].noteType = NoteType.NONE;
            noteArray[idx].waitingUnit = 0;
        }

        for (int i = idx + 1; i < LoadedSectionList.Count && i < noteArray.Length; i++) {

            noteArray[i].effectTimeUnit = 0;
            noteArray[i].noteType = NoteType.NONE;
            noteArray[i].waitingUnit = 0;
        }

        btn_Load_Section();
        btn_BGM_SectionPlay();

    }

    public void btn_ClearSection() {

        btn_Load_Section();

        for (int i = 0; i < LoadedSectionList.Count; i++) { 
            LoadedSectionList[i].SetNoteType(NoteType.NONE);
        }

        btn_Save_Section();

        btn_BGM_Pause();

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
        audioPlayer.time = 0;


        float audioLength = audioPlayer.clip.length;

        audioPlayer.Play();

        yield return new WaitForSeconds(_offset);


        int flagcnter = 0;

        while (audioPlayer.isPlaying || audioPlayer.time < audioLength) {

            if (audioPlayer.time > cmpTime)
            {
                cmpTime += _unitBPMSecond;

                
                if (noteArray[cnt].effectTimeUnit != 0) {

                    if (flagcnter % 2 == 0) {

                        
                        display_Button(cnt);

                    }

                    flagcnter++;

                } Debug.Log("넘기는 마디 수 : " + noteArray[cnt].effectTimeUnit);

                StartCoroutine(PlayNote(noteArray[cnt]));

                debugText.text = "curCnt : " + cnt + " / arrLength : " + noteArray.Length;


                cnt++;

            }

            yield return null;
        }

    }


    #region 임시 디스플레이 전용 코루틴

    public void display_Button(int _cnt)
    {

        int startIdx = _cnt;
        int endIdx = _cnt + noteArray[_cnt].effectTimeUnit * BPM_Multiplyer * 4;

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        Array.Copy(noteArray, startIdx, sectionArr, 0, endIdx - startIdx);

        LoadedSectionList = BoarderSystem.SetNotes(sectionArr, BPM_Multiplyer);

        StartCoroutine(ButtonDisplayer());

    }

    private IEnumerator ButtonDisplayer()
    {

        ClearBoarderImg();

        int cnt = 0;

        double cmpTime = audioPlayer.time;

        while (cnt < LoadedSectionList.Count)
        {
            if (audioPlayer.time > cmpTime)
            {
                cmpTime += unitBPMSecond;
                LoadedSectionList[cnt].SetBoarderImg(true);
                cnt++;
            }

            yield return null;
        }

    }

#endregion



    public enum NoteType { NONE, DOWN_NOTE, UPPER_NOTE, INVERSE_DOWN_NOTE, INVERSE_UPPER_NOTE};
    private IEnumerator PlayNote(NoteInfo _noteInfo) {


        switch (_noteInfo.noteType) {

            case NoteType.DOWN_NOTE:
                audioPlayer.PlayOneShot(downFXClip);
                StartCoroutine(WaitInput(downFXClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.UPPER_NOTE:
                audioPlayer.PlayOneShot(upperFXClip);
                StartCoroutine(WaitInput(upperFXClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.INVERSE_DOWN_NOTE:
                audioPlayer.PlayOneShot(downFXClip);
                StartCoroutine(WaitInput(upperFXClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.INVERSE_UPPER_NOTE:
                audioPlayer.PlayOneShot(upperFXClip);
                StartCoroutine(WaitInput(downFXClip, _noteInfo.waitingUnit, true));
                break;

            default:
                yield return null;
                break;

        }

    }

    //todo : 판정 확인점으로 바꿔야 하는 함수, playoneshot을 이용하면 pause가 풀림. 이걸 우회할 방법 고려.!!
    //todo : 채보시에는 필요 없음!!!
    private IEnumerator WaitInput(AudioClip _snd, int _waitUnitTime, bool _isAuto) {

        float curTime = audioPlayer.time;
        float destTime = curTime + (float)(unitBPMSecond * BPM_Multiplyer * _waitUnitTime);

        while (curTime < destTime) {


            yield return null;

            if(audioPlayer.isPlaying) curTime += Time.deltaTime;


        }

        if(_isAuto) audioPlayer.PlayOneShot(_snd);

    }



    internal struct NoteInfo {

        public NoteType noteType;
        public int waitingUnit;
        public int effectTimeUnit;

    }

    private NoteInfo[] noteArray = null;

    public void InitNoteSheet() {

        if (bgmClip == null) return;

        float bgmTime = bgmClip.length;
        Debug.Log(bgmTime);
        float minOfLength = bgmTime / 60f;//1.666666666min

        int beatInBGM = (int)Mathf.Ceil(minOfLength * (int)BPM * BPM_Multiplyer);//노랫속 beat의 총 갯수

        int arrCount = beatInBGM + BPM_Multiplyer;


        if (noteArray == null)
        {

            noteArray = new NoteInfo[arrCount];


        }



    }

    public void ClearNoteSheet() {
        noteArray = null;
    }



    public AudioClip bgmClip;
    public AudioClip downFXClip;
    public AudioClip upperFXClip;

}
