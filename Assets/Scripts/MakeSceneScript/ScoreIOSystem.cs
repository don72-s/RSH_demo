using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIOSystem : MonoBehaviour {

    [Header("NoteBlocks Display")]
    [SerializeField]
    GridLayoutGroup upperScoreBoard;
    List<ButtonScript> upperBtnList;
    [SerializeField]
    GridLayoutGroup lowerScoreBoard;
    List<ButtonScript> lowerBtnList;


    [Header("Note Prefabs")]
    [SerializeField]
    ButtonScript upperNotePrefab;
    [SerializeField]
    ButtonScript lowerNotePrefab;

    // Start is called before the first frame update
    void Start() {

        upperBtnList = new List<ButtonScript>();
        lowerBtnList = new List<ButtonScript>();

        //위, 아래 모두 16개를 기본으로 오브젝트 풀 조정.
        for (int i = 0; i < 16; i++) {

            upperBtnList.Add(Instantiate(upperNotePrefab, upperScoreBoard.transform));
            lowerBtnList.Add(Instantiate(lowerNotePrefab, lowerScoreBoard.transform));

        }

        foreach (ButtonScript _btnScr in upperBtnList) _btnScr.gameObject.SetActive(false);
        foreach (ButtonScript _btnScr in lowerBtnList) _btnScr.gameObject.SetActive(false);

    }




    /// <summary>
    /// 노드 타입 정보를 받아서 위, 아래 디스플레이 정보를 세팅.
    /// </summary>
    /// <param name="_arr">노트 타입 배열</param>
    /// <param name="_bpmMultiplyer">세분화 비트 단위</param>
    /// <returns>디스플래이 된 ButtonScript 객체들의 리스트를 반환.</returns>
    public List<ButtonScript> SetNotes(NoteType[] _arr, int _bpmMultiplyer) {

        List<ButtonScript> retButtonList = new List<ButtonScript>();

        DisplayNotes(upperBtnList, upperScoreBoard, _arr, upperNotePrefab, _bpmMultiplyer, retButtonList);
        DisplayNotes(lowerBtnList, lowerScoreBoard, _arr, lowerNotePrefab, _bpmMultiplyer, retButtonList);

        return retButtonList;
    }

    /// <summary>
    /// 노드 정보를 받아서 위, 아래 디스플레이 정보를 세팅.
    /// </summary>
    /// <param name="_arr">노트 정보 배열</param>
    /// <param name="_bpmMultiplyer">세분화 비트 단위</param>
    /// <returns>디스플래이 된 ButtonScript 객체들의 리스트를 반환.</returns>
    internal List<ButtonScript> SetNotes(NoteInfo[] _arr, int _bpmMultiplyer) {

        NoteType[] _noteTypeArr = new NoteType[_arr.Length];

        for (int i = 0; i < _noteTypeArr.Length; i++) {

            _noteTypeArr[i] = _arr[i].noteType;

        }

        return SetNotes(_noteTypeArr, _bpmMultiplyer);

    }


    /// <summary>
    /// 실제 노트의 디스플레이를 세팅
    /// </summary>
    /// <param name="_buttonsList">대상이 될 디스플레이 버튼들의 리스트</param>
    /// <param name="_parentObject">해당 버튼들의 부모 오브젝트</param>
    /// <param name="_arr">세팅할 노트 정보 배열</param>
    /// <param name="_btnObject">오브젝트 풀이 부족할 시, 추가 객체화 할 인스턴스</param>
    /// <param name="_rowCount">조정할 가로 갯수의 크기</param>
    /// <param name="_retList">반환할 최종 버튼 객체들의 리스트</param>
    private void DisplayNotes(List<ButtonScript> _buttonsList, GridLayoutGroup _parentObject, NoteType[] _arr, ButtonScript _btnObject, int _rowCount, List<ButtonScript> _retList) {

        int cnt = _arr.Length;

        foreach (ButtonScript _scr in _buttonsList) {

            _scr.gameObject.SetActive(false);

        }

        for (int i = 0; i < cnt; i++) {

            if (_buttonsList.Count <= i) {

                _buttonsList.Add(Instantiate(_btnObject, _parentObject.transform));

            }

            _buttonsList[i].gameObject.SetActive(true);
            _buttonsList[i].SetNoteType(_arr[i]);
            _retList.Add(_buttonsList[i]);

        }

        //가로 길이 조정.
        _parentObject.constraintCount = _rowCount;
        _parentObject.cellSize = new Vector2(400 / _rowCount, 50);

    }


}
