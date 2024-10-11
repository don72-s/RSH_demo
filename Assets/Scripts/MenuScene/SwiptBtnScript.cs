using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SwiptBtnScript : MonoBehaviour {


    int myIdx;
    float unitDistance;

    float smallSize = 0.8f;

    Action<int> myBtnClick = null;

    [SerializeField]
    Button deleteBtn;
    [SerializeField]
    Text childText;

    AlertWindow window;

    private void Awake() {

        GetComponent<Button>().onClick.AddListener(() => myBtnClick?.Invoke(myIdx));
        deleteBtn.onClick.AddListener(DeleteStage);

    }


    /// <summary>
    /// 데이터 초기 설정
    /// </summary>
    /// <param name="_window">경고창을 띄우기 위한 경고창 객체</param>
    /// <param name="_btnClickedCallback">스테이지 버튼이 눌렸을 경우 실행할 함수</param>
    public void InitData(AlertWindow _window, Action<int> _btnClickedCallback) {

        window = _window;
        myBtnClick = _btnClickedCallback;
    }

    /// <summary>
    /// 버튼들이 재배치될때마다 실행될 함수.
    /// </summary>
    /// <param name="_idx">해당 버튼의 idx설정</param>
    /// <param name="_unitDistance">버튼간 거리 설정</param>
    public void SetData(int _idx, float _unitDistance) {

        myIdx = _idx;
        unitDistance = _unitDistance;

    }

    /// <summary>
    /// 0~1 사이의 스크롤바 값이 들어옴, 가운데 버튼이 크게 보이는 효과.
    /// </summary>
    /// <param name="_barValue"></param>
    public void SetButtonSize(float _barValue) {

        float localUnitDistance = Mathf.Abs(Mathf.Clamp(_barValue - (myIdx * 2 * unitDistance), -unitDistance * 2, unitDistance * 2));

        float localValue = 1 - ((localUnitDistance / (unitDistance * 2)) * (1 - smallSize));

        transform.localScale = new Vector2(localValue, localValue);

    }

    /// <summary>
    /// 버튼에 출력될 내용과 x버튼 여부 설정
    /// </summary>
    /// <param name="_text">출력될 문자열([스테이지 파일 이름] - [.dat])</param>
    public void SetButtonDescription(string _text) {

        childText.text = _text;

        if (_text.StartsWith("stage") || _text == "+") {

            deleteBtn.gameObject.SetActive(false);

        }
    }

    public string GetText() {

        return childText.text;

    }

    /// <summary>
    /// 스테이지 삭제버튼이 눌렸을 때 띄울 메세지와 선택지.
    /// </summary>
    void DeleteStage() {

        window.ShowDoubleAlertWindow("정말로 해당 스테이지를 삭제하시겠습니까?", "네", () => {

            //'네'가 눌렸을 경우 파일이 있으면 스테이지를 삭제하고 리로드.
            if (NoteDataManager.AndroidDeleteFile($"{childText.text}.dat")) {

                SceneManager.LoadScene("MenuScene");

            } else {

                window.ShowSingleAlertWindow("스테이지 파일이 존재하지 않습니다.[error]");

            }

        });

    }

}
