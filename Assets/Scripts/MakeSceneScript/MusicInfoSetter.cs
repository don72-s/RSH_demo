using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicInfoSetter : MonoBehaviour {

    AudioSource audioPlayer;

    AudioClip bgmClip;
    AudioClip upperSEClip;
    AudioClip lowerSEClip;

    BGM_TYPE bgmType;
    SE_TYPE upperSeType;
    SE_TYPE lowerSeType;

    [Header("SFX's SO")]
    [SerializeField]
    AudioDictonary audioClipDic;

    [Header("ScoreBoard UI")]
    [SerializeField]
    ScoreIOSystem BoarderSystem;
    [SerializeField]
    SectionInfoDis sectionDisplayer;

    [Header("Alert Window")]
    [SerializeField]
    AlertWindow alertWindow;

    [Header("Export Window")]
    [SerializeField]
    GameObject exportWindow;

    [Header("Section Set Input Fields")]
    [SerializeField]
    InputField sectionNumInputter;
    [SerializeField]
    InputField sectionLengthInputter;

    [Header("Section State Text")]
    [SerializeField]
    Text stateText;

    [Header("Create UI Dropdown")]
    [Space(15)]
    [SerializeField]
    BGMDropdown BGM_Dropdown;
    [SerializeField]
    SEDropdown LowerSE_Dropdown;
    [SerializeField]
    SEDropdown UpperSE_Dropdown;

    [Header("Create UI InputField")]
    [SerializeField]
    InputField BPM_Multiplyer_Inputfield;
    [SerializeField]
    InputField ScoreUnit_Inputfield;

    List<ButtonScript> LoadedSectionList = null;

    string saveFileName;

    NoteInfo[] noteArray = null;


    // Start is called before the first frame update
    void Start() {

        audioPlayer = GetComponent<AudioSource>();

    }


    float offsetSecond = 0;  //싱크를 위한 앞부분 대기 시간.
    float BPM;               //곡의 bpm [ 노래 자체 고유값, 임의 변경은 거의 불가능 ]
    int BPM_Multiplyer;      //한마디의 큰 비트는 4개[기본], 각 큰 비트 사이를 몇개의 세분 비트로 쪼갤지 결정하는 입력변수.

    int scoreUnit = 4;

    double unitBPMSecond;

    public void Btn_TestPlay() {

        StopAllCoroutines();

        if (noteArray == null) { alertWindow.ShowSingleAlertWindow("note map doesn't exist. \n Please load or create it first."); return; }

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        sectionDisplayer.ClearFocus();

        StartCoroutine(playBeatTest(offsetSecond, unitBPMSecond, BPM_Multiplyer));

    }

    /// <summary>
    /// 불러오고 맵 초기설정
    /// </summary>
    public StageInfo LoadNoteData(string _fileName) {

        StageInfo stageInfo = FileIOSystem.LoadData(_fileName);

        if (stageInfo == null) return null;

        offsetSecond = stageInfo.offsetSecond;
        BPM = stageInfo.bpm;
        BPM_Multiplyer = stageInfo.bpmMultiplier;
        scoreUnit = stageInfo.scoreUnit;

        noteArray = stageInfo.noteArray;

        bgmClip = audioClipDic.GetBGMClip(stageInfo.bgmType);

        upperSeType = stageInfo.upperSeType;
        lowerSeType = stageInfo.lowerSeType;

        upperSEClip = audioClipDic.GetSEClip(stageInfo.upperSeType);
        lowerSEClip = audioClipDic.GetSEClip(stageInfo.lowerSeType);

        return stageInfo;

    }


    public void Btn_BGM_Pause() {

        if (audioPlayer.clip == null) return;

        if (audioPlayer.isPlaying) {

            audioPlayer.Pause();

        } else {
            audioPlayer.UnPause();
        }

    }

    public void PlaySection() {

        sectionDisplayer.ClearFocus();

        StopAllCoroutines();

        if (LoadedSectionList == null) { alertWindow.ShowSingleAlertWindow("Section not loaded.\n Please load the section to be edited first."); return; }

        unitBPMSecond = (double)60 / (BPM * BPM_Multiplyer);

        if (!InputChecker.IsPositiveInt(sectionNumInputter, true)) { alertWindow.ShowSingleAlertWindow("sectionNum must be positive number or 0."); return; }
        if (!InputChecker.IsPositiveInt(sectionLengthInputter)) { alertWindow.ShowSingleAlertWindow("sectionLength must be positive number."); return; }

        int sectionNum = InputChecker.GetInt(sectionNumInputter);
        int sectionLength = InputChecker.GetInt(sectionLengthInputter);


        float startTime = (float)(sectionNum * BPM_Multiplyer * scoreUnit * unitBPMSecond);
        float duringTime = (float)(sectionLength * BPM_Multiplyer * scoreUnit * 2 * unitBPMSecond);

        int startIdx = sectionNum * BPM_Multiplyer * scoreUnit;

        StartCoroutine(playSection(startIdx, offsetSecond, startTime, startTime + duringTime, unitBPMSecond));

    }

    public void Btn_Load_Section() {

        LoadSection();

    }

    public bool LoadSection() {

        StopAllCoroutines();

        if (noteArray == null) { alertWindow.ShowSingleAlertWindow("note map doesn't exist. \n Please load or create it first."); return false; }

        if (!InputChecker.IsPositiveInt(sectionNumInputter, true)) { alertWindow.ShowSingleAlertWindow("sectionNum must be positive number or 0."); return false; }
        if (!InputChecker.IsPositiveInt(sectionLengthInputter)) { alertWindow.ShowSingleAlertWindow("sectionLength must be positive number."); return false; }

        int sectionNum = InputChecker.GetInt(sectionNumInputter);
        int sectionLength = InputChecker.GetInt(sectionLengthInputter);

        int startIdx = sectionNum * BPM_Multiplyer * scoreUnit;
        int endIdx = startIdx + sectionLength * BPM_Multiplyer * scoreUnit;

        int curEndIdx = startIdx + ((endIdx - startIdx) * 2);
        if (curEndIdx > noteArray.Length) { alertWindow.ShowSingleAlertWindow("cannot notate after the music has finished."); return false; }

        stateText.text = $"Section {{ {sectionNum} ~ {(sectionNum + (sectionLength * 2) - 1) } }} [ L : {sectionLength} ]";

        sectionDisplayer.SetupSectionDisplay(noteArray);

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        Array.Copy(noteArray, startIdx, sectionArr, 0, endIdx - startIdx);

        LoadedSectionList = BoarderSystem.SetNotes(sectionArr, BPM_Multiplyer);

        ClearBoarderImg();

        return true;

    }


    public void Btn_Save_NoteMap(InputField _fileNameInputField) {

        if (noteArray == null) { alertWindow.ShowSingleAlertWindow("note map doesn't exist. \n Please load or create it first."); return; }

        saveFileName = _fileNameInputField.text;

        if (saveFileName.StartsWith("stage")) { alertWindow.ShowSingleAlertWindow("file name cannot start with 'stage'"); return; }
        if (!saveFileName.EndsWith(".dat")) { alertWindow.ShowSingleAlertWindow("file name must end with [.dat]"); return; }
        if (saveFileName.Equals(".dat")) { alertWindow.ShowSingleAlertWindow("file name cannot be empty."); return; }


        if (FileIOSystem.CheckFileExist(saveFileName)) {
            alertWindow.ShowDoubleAlertWindow($"[ {saveFileName} ] already exists.\nDo you want to overwrite it?", "OK!", () => {

                FileIOSystem.SaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType, saveFileName);
                alertWindow.ShowSingleAlertWindow($"file [ {saveFileName} ] has been saved!");
                exportWindow.SetActive(false);

            });
        } else {
            FileIOSystem.SaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType, saveFileName);
            alertWindow.ShowSingleAlertWindow($"file [ {saveFileName} ] has been saved!");
            exportWindow.SetActive(false);

        }

    }

    /// <summary>
    /// 현재 임시 노트인 noteArray[]에 구간채보 적용
    /// </summary>
    public void Btn_Apply_Section() {

        if (LoadedSectionList == null) { alertWindow.ShowSingleAlertWindow("Section not loaded.\n Please load the section to be edited first."); return; }

        if (!InputChecker.IsPositiveInt(sectionNumInputter, true)) { alertWindow.ShowSingleAlertWindow("sectionNum must be positive number or 0."); return; }
        if (!InputChecker.IsPositiveInt(sectionLengthInputter)) { alertWindow.ShowSingleAlertWindow("sectionLength must be positive number."); return; }

        int sectionNum = InputChecker.GetInt(sectionNumInputter);
        int sectionLength = InputChecker.GetInt(sectionLengthInputter);

        int idx = sectionNum * BPM_Multiplyer * scoreUnit;


        for (int i = 0; i < LoadedSectionList.Count / 2; i++) {

            NoteType type = LoadedSectionList[i].GetNoteType();

            noteArray[idx].noteType = type;
            noteArray[idx].waitingUnit = sectionLength * scoreUnit;
            noteArray[idx].waitScoreCount = 0;

            idx++;

        }

        noteArray[sectionNum * BPM_Multiplyer * scoreUnit].waitScoreCount = sectionLength;

        if (idx < noteArray.Length) {
            noteArray[idx].waitScoreCount = sectionLength;
            noteArray[idx].noteType = NoteType.NONE;
            noteArray[idx].waitingUnit = 0;
        }

        for (int i = idx + 1; i - idx < LoadedSectionList.Count / 2 && i < noteArray.Length; i++) {

            noteArray[i].waitScoreCount = 0;
            noteArray[i].noteType = NoteType.NONE;
            noteArray[i].waitingUnit = 0;

        }

        LoadSection();
        PlaySection();

    }

    public void Btn_ClearSection() {

        if (!LoadSection()) return;

        for (int i = 0; i < LoadedSectionList.Count; i++) {
            LoadedSectionList[i].SetNoteType(NoteType.NONE);
        }

        Btn_Apply_Section();
        Btn_BGM_Pause();

    }


    public void ClearBoarderImg() {

        if (LoadedSectionList == null) { alertWindow.ShowSingleAlertWindow("Section not loaded.\n Please load the section to be edited first."); return; }

        foreach (ButtonScript _btnScr in LoadedSectionList) {

            _btnScr.SetBoarderImgActive(false);

        }

    }


    /// <summary>
    /// 지정 구간 재생
    /// </summary>
    /// <param name="_startIdx"></param>
    /// <param name="_offset"></param>
    /// <param name="_startTime"></param>
    /// <param name="_endTime"></param>
    /// <param name="_unitBPMSecond"></param>
    /// <returns></returns>
    private IEnumerator playSection(int _startIdx, float _offset, float _startTime, float _endTime, double _unitBPMSecond) {

        ClearBoarderImg();

        int noteCnt = _startIdx;
        int btnCnt = 0;

        double cmpTime = _startTime + _offset;
        double endTime = _endTime + _offset;

        audioPlayer.clip = bgmClip;
        audioPlayer.time = _startTime;
        audioPlayer.Play();


        while (audioPlayer.isPlaying && audioPlayer.time < endTime) {

            if (audioPlayer.time > cmpTime) {
                cmpTime += _unitBPMSecond;

                StartCoroutine(PlayNote(noteArray[noteCnt]));

                if (btnCnt < LoadedSectionList.Count) LoadedSectionList[btnCnt].SetBoarderImgActive(true);

                noteCnt++;
                btnCnt++;

            }

            yield return null;
        }

        audioPlayer.Stop();

    }


    /// <summary>
    /// 전체 구간 재생.
    /// </summary>
    /// <param name="_offset"></param>
    /// <param name="_unitBPMSecond"></param>
    /// <param name="_bpmMultiCount"></param>
    /// <returns></returns>
    private IEnumerator playBeatTest(float _offset, double _unitBPMSecond, int _bpmMultiCount) {

        double cmpTime = _offset;
        int cnt = 0;

        audioPlayer.clip = bgmClip;
        audioPlayer.time = 0;


        float audioLength = audioPlayer.clip.length;

        audioPlayer.Play();
        stateText.text = "Playing...";

        yield return new WaitForSeconds(_offset);


        int flagcnter = 0;

        while (audioPlayer.isPlaying || audioPlayer.time < audioLength) {

            if (audioPlayer.time > cmpTime) {
                cmpTime += _unitBPMSecond;


                if (noteArray[cnt].waitScoreCount != 0) {

                    if (flagcnter % 2 == 0) {

                        Display_Note(cnt);
                        sectionDisplayer.SetFocus(flagcnter / 2);

                    }

                    flagcnter++;

                }

                StartCoroutine(PlayNote(noteArray[cnt]));


                cnt++;

            }

            yield return null;
        }

    }


    #region 디스플레이 전용 코루틴

    public void Display_Note(int _cnt) {

        int startIdx = _cnt;
        int endIdx = _cnt + noteArray[_cnt].waitScoreCount * BPM_Multiplyer * scoreUnit;

        NoteInfo[] sectionArr = new NoteInfo[endIdx - startIdx];

        if (noteArray.Length < startIdx + endIdx - startIdx) return;

        Array.Copy(noteArray, startIdx, sectionArr, 0, endIdx - startIdx);

        LoadedSectionList = BoarderSystem.SetNotes(sectionArr, BPM_Multiplyer);

        StartCoroutine(ButtonDisplayer());

    }

    private IEnumerator ButtonDisplayer() {

        ClearBoarderImg();

        int cnt = 0;

        double cmpTime = audioPlayer.time;

        while (cnt < LoadedSectionList.Count) {
            if (audioPlayer.time > cmpTime) {
                cmpTime += unitBPMSecond;
                LoadedSectionList[cnt].SetBoarderImgActive(true);
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


    private IEnumerator WaitInput(AudioClip _snd, int _waitUnitTime, bool _isAuto) {

        float curTime = audioPlayer.time;
        float destTime = curTime + (float)(unitBPMSecond * BPM_Multiplyer * _waitUnitTime);

        while (curTime < destTime) {


            yield return null;

            if (audioPlayer.isPlaying) curTime += Time.deltaTime;


        }

        if (_isAuto) audioPlayer.PlayOneShot(_snd);

    }



    public void Btn_Create_New_Score(GameObject _loadWindow) {

        if (!InputChecker.IsPositiveInt(BPM_Multiplyer_Inputfield)) { alertWindow.ShowSingleAlertWindow("BPM Multiplyer must be a positive number."); return; }
        if (!InputChecker.IsPositiveInt(ScoreUnit_Inputfield)) { alertWindow.ShowSingleAlertWindow("Score Unit must be a positive number. "); return; }

        BPM_Multiplyer = InputChecker.GetInt(BPM_Multiplyer_Inputfield);
        scoreUnit = InputChecker.GetInt(ScoreUnit_Inputfield);

        bgmType = BGM_Dropdown.GetItem();
        lowerSeType = LowerSE_Dropdown.GetItem();
        upperSeType = UpperSE_Dropdown.GetItem();

        bgmClip = audioClipDic.GetBGMClip(bgmType);
        audioPlayer.clip = bgmClip;
        upperSEClip = audioClipDic.GetSEClip(upperSeType);
        lowerSEClip = audioClipDic.GetSEClip(lowerSeType);

        offsetSecond = audioClipDic.GetBGMOffset(bgmType);
        BPM = audioClipDic.GetBGM_BPM(bgmType);

        float bgmTime = bgmClip.length;
        float minOfLength = bgmTime / 60f;

        int beatInBGM = (int)Mathf.Ceil(minOfLength * (int)BPM * BPM_Multiplyer);//노랫속 beat의 총 갯수

        int arrCount = beatInBGM + BPM_Multiplyer;


        noteArray = new NoteInfo[arrCount];

        for (int i = 0; i < arrCount; i++) {
            noteArray[i] = new NoteInfo();
        }


        alertWindow.ShowSingleAlertWindow("create success!");
        _loadWindow.SetActive(false);
        sectionDisplayer.SetupSectionDisplay(noteArray);

    }

}
