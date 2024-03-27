using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MusicInfoSetter : MonoBehaviour
{

    AudioSource audioPlayer;

    AudioClip bgmClip;
    AudioClip upperSEClip;
    AudioClip lowerSEClip;


    [SerializeField]
    BGM_TYPE bgmType;
    [SerializeField]
    SE_TYPYE upperSeType;
    [SerializeField]
    SE_TYPYE lowerSeType;

    [SerializeField]
    AudioDictonary audioClipDic;


    [SerializeField]
    ScoreIOSystem BoarderSystem;
    [SerializeField]
    SectionInfoDis sectionInfoDisplayer;

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

    public int scoreUnit = 4;


    double unitBPMSecond;

    public void btn_TestPlay() { 
    
        StopAllCoroutines();

        InitNoteSheet();

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer) ;

        sectionInfoDisplayer.ClearFocus();

        StartCoroutine(playBeatTest(offsetSecond, unitBPMSecond, BPM_Multiplyer));

    }


    public string loadFileName = "stage1Note.dat";


    /// <summary>
    /// 불러오고 맵 초기설정 초반 버튼용 디버그용
    /// </summary>
    public void btn_Setup_And_LoadNoteData() {
        //todo : 기본 맵 가져오는거? ㄱㄴ but 기본맵과 같은 이름으로 덮어씌워 저장? => 불가능!!
#if UNITY_EDITOR
        StageInfo stageInfo = NoteDataManager.LoadData(loadFileName);
#elif UNITY_ANDROID
        StageInfo stageInfo = NoteDataManager.AndroidLoadData("stage1Note.dat");
#endif

        if (stageInfo == null) return;

        offsetSecond = stageInfo.offsetSecond;
        BPM = stageInfo.bpm;
        BPM_Multiplyer = stageInfo.bpmMultiplier;
        scoreUnit = stageInfo.scoreUnit;
        
        noteArray = stageInfo.noteArray;

        bgmClip = audioClipDic.GetBGMClip(stageInfo.bgmType);
        upperSEClip = audioClipDic.GetSEClip(stageInfo.upperSeType);
        lowerSEClip = audioClipDic.GetSEClip(stageInfo.lowerSeType);

    }


    /// <summary>
    /// 불러오고 맵 초기설정
    /// </summary>
    public StageInfo LoadNoteData(string _fileName)
    {
        //todo : 기본 맵 가져오는거? ㄱㄴ but 기본맵과 같은 이름으로 덮어씌워 저장? => 불가능!!
#if UNITY_EDITOR
        StageInfo stageInfo = NoteDataManager.LoadData(_fileName);
#elif UNITY_ANDROID
        StageInfo stageInfo = NoteDataManager.AndroidLoadData(_fileName);
#endif

        if (stageInfo == null) return null;

        offsetSecond = stageInfo.offsetSecond;
        BPM = stageInfo.bpm;
        BPM_Multiplyer = stageInfo.bpmMultiplier;
        scoreUnit = stageInfo.scoreUnit;

        noteArray = stageInfo.noteArray;

        bgmClip = audioClipDic.GetBGMClip(stageInfo.bgmType);
        upperSEClip = audioClipDic.GetSEClip(stageInfo.upperSeType);
        lowerSEClip = audioClipDic.GetSEClip(stageInfo.lowerSeType);

        return stageInfo;

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

        sectionInfoDisplayer.ClearFocus();

        StopAllCoroutines();

        if (LoadedSectionList == null) return;

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        float startTime = (float) (int.Parse(sectionNumInputter.text) * BPM_Multiplyer * scoreUnit * unitBPMSecond);
        float duringTime = (float)(int.Parse(sectionLengthInputter.text) * BPM_Multiplyer * scoreUnit * 2 * unitBPMSecond);


        debugText.text = "curSection : " + int.Parse(sectionNumInputter.text) + " ~ " + (int.Parse(sectionNumInputter.text) + (int.Parse(sectionLengthInputter.text) * 2) - 1);

        int startIdx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * scoreUnit;

        StartCoroutine(playSection(startIdx, offsetSecond, startTime, startTime + duringTime, unitBPMSecond));
    }


    private List<ButtonScript> LoadedSectionList = null;

    public void btn_Load_Section()
    {

        StopAllCoroutines();

        //test
        //unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        int startIdx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * scoreUnit;
        int endIdx = startIdx + int.Parse(sectionLengthInputter.text) * BPM_Multiplyer * scoreUnit;


        debugText.text = "LoadedSection : " + int.Parse(sectionNumInputter.text) + " ~ " + (int.Parse(sectionNumInputter.text) + (int.Parse(sectionLengthInputter.text) * 2) - 1);

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        Array.Copy(noteArray, startIdx, sectionArr, 0, endIdx - startIdx);

        LoadedSectionList = BoarderSystem.SetNotes(sectionArr, BPM_Multiplyer);

        ClearBoarderImg();

    }



    public void btn_expertScore() {

        if (noteArray == null) return;

#if UNITY_EDITOR

        NoteDataManager.SaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType, saveFileName);

#elif UNITY_ANDROID

        NoteDataManager.AndroidSaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType);

#endif


    }

    public string saveFileName = "st1";

    public void btn_Save_Section() {

        if (LoadedSectionList == null) return;

        int idx = int.Parse(sectionNumInputter.text) * BPM_Multiplyer * scoreUnit;

        for (int i = 0; i < LoadedSectionList.Count / 2; i++) {

            NoteType type = LoadedSectionList[i].GetNoteType();

            noteArray[idx].noteType = type;
            noteArray[idx].waitingUnit = int.Parse(sectionLengthInputter.text) * scoreUnit;
            noteArray[idx].waitScoreCount = 0;

            idx++;

        }

        //첫 노트에 대기 단위 추가
        noteArray[int.Parse(sectionNumInputter.text) * BPM_Multiplyer * scoreUnit].waitScoreCount = int.Parse(sectionLengthInputter.text);

        if (idx < noteArray.Length)
        {
            Debug.Log("idx : " + idx);
            noteArray[idx].waitScoreCount = int.Parse(sectionLengthInputter.text);
            noteArray[idx].noteType = NoteType.NONE;
            noteArray[idx].waitingUnit = 0;
        }

        for (int i = idx + 1; i < LoadedSectionList.Count && i < noteArray.Length; i++) {

            noteArray[i].waitScoreCount = 0;
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

                
                if (noteArray[cnt].waitScoreCount != 0) {

                    if (flagcnter % 2 == 0) {

                        display_Button(cnt);
                        sectionInfoDisplayer.SetFocus(flagcnter / 2);

                    }

                    flagcnter++;

                }

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
        int endIdx = _cnt + noteArray[_cnt].waitScoreCount * BPM_Multiplyer * scoreUnit;

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        if (noteArray.Length < startIdx + endIdx - startIdx) return;

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

    private IEnumerator PlayNote(NoteInfo _noteInfo) {


        switch (_noteInfo.noteType) {

            case NoteType.DOWN_NOTE:
                audioPlayer.PlayOneShot(lowerSEClip);
                StartCoroutine(WaitInput(lowerSEClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.UPPER_NOTE:
                audioPlayer.PlayOneShot(upperSEClip);
                StartCoroutine(WaitInput(upperSEClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.INVERSE_DOWN_NOTE:
                audioPlayer.PlayOneShot(lowerSEClip);
                StartCoroutine(WaitInput(upperSEClip, _noteInfo.waitingUnit, true));
                break;

            case NoteType.INVERSE_UPPER_NOTE:
                audioPlayer.PlayOneShot(upperSEClip);
                StartCoroutine(WaitInput(lowerSEClip, _noteInfo.waitingUnit, true));
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


    private NoteInfo[] noteArray = null;

    public void InitNoteSheet() {

        if (bgmClip == null) { 
        
            bgmClip = audioClipDic.GetBGMClip(bgmType);
            upperSEClip = audioClipDic.GetSEClip(upperSeType);
            lowerSEClip = audioClipDic.GetSEClip(lowerSeType);

        }

        float bgmTime = bgmClip.length;
        Debug.Log(bgmTime);
        float minOfLength = bgmTime / 60f;//1.666666666min

        int beatInBGM = (int)Mathf.Ceil(minOfLength * (int)BPM * BPM_Multiplyer);//노랫속 beat의 총 갯수

        int arrCount = beatInBGM + BPM_Multiplyer;


        if (noteArray == null)
        {

            noteArray = new NoteInfo[arrCount];

            for (int i = 0; i < arrCount; i++) {
                noteArray[i] = new NoteInfo();
            }
            
        }



    }

    public void ClearNoteSheet() {
        noteArray = null;
    }

}
