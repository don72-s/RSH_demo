using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIOSystem : MonoBehaviour
{

    [SerializeField]
    private GridLayoutGroup upperScoreBoard;
    private List<ButtonScript> upperBtnList;

    [SerializeField]
    private GridLayoutGroup lowerScoreBoard;
    private List<ButtonScript> lowerBtnList;



    [SerializeField]
    private GameObject inputUpperBtnObj;

    [SerializeField]
    private GameObject inputLowerBtnObj;

    // Start is called before the first frame update
    void Start()
    {

        upperBtnList = new List<ButtonScript>();
        lowerBtnList = new List<ButtonScript>();

        for (int i = 0; i < 16; i++) {

            upperBtnList.Add(Instantiate(inputUpperBtnObj, upperScoreBoard.transform).GetComponent<ButtonScript>());
            lowerBtnList.Add(Instantiate(inputLowerBtnObj, lowerScoreBoard.transform).GetComponent<ButtonScript>());

        }

    }



    private List<ButtonScript> retButtonList;

    //todo : 범위를 tite하게 지정해야 함. => 구간을 출력하는거기 때문. 매개변수 추가? or 범위만큼만 세팅? && 가로 버튼 갯수 : bpm multiplyer와 같은 숫자로 받아서 설정
    public List<ButtonScript> SetNotes(NoteType[] _arr, int _bpmMultiplyer) {

        retButtonList = new List<ButtonScript>();

        SetButtonUI(upperBtnList, upperScoreBoard, _arr, inputUpperBtnObj, _bpmMultiplyer, retButtonList);
        SetButtonUI(lowerBtnList, lowerScoreBoard, _arr, inputLowerBtnObj, _bpmMultiplyer, retButtonList);

        return retButtonList;
    }

    internal List<ButtonScript> SetNotes(NoteInfo[] _arr, int _bpmMultiplyer)
    {

        NoteType[] _noteTypeArr = new NoteType[_arr.Length];

        for (int i = 0; i < _noteTypeArr.Length; i++) {
            _noteTypeArr[i] = _arr[i].noteType;
        }

        return SetNotes(_noteTypeArr, _bpmMultiplyer);

    }

    private void SetButtonUI(List<ButtonScript> _buttonsList, GridLayoutGroup _parentObject, NoteType[] _arr, GameObject _btnObject, int _rowCount, List<ButtonScript> _retList) {

        int cnt = _arr.Length;

        foreach (ButtonScript _scr in _buttonsList)
        {
            _scr.gameObject.SetActive(false);
        }

        for (int i = 0; i < cnt; i++)
        {

            if (_buttonsList.Count <= i)
            {
                _buttonsList.Add(Instantiate(_btnObject, _parentObject.transform).GetComponent<ButtonScript>());
            }

            _buttonsList[i].gameObject.SetActive(true);
            _buttonsList[i].SetNoteType(_arr[i]);
            _retList.Add(_buttonsList[i]);

        }

        //todo : 가로갯수 조정
        _parentObject.constraintCount = _rowCount;
        _parentObject.cellSize = new Vector2(400 / _rowCount, 50);
    }


}
