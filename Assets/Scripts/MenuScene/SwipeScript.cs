using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwipeScript : MonoBehaviour
{

    delegate void FocusSizeDelegate(float _scrollValue);



    [SerializeField]
    Scrollbar scrollBar;

    [SerializeField]
    Transform ButtonParent;

    [SerializeField]
    GameObject ButtonInstance;


    FocusSizeDelegate focusSizeDelegate = null;

    float curPos = 0;
    float destPos = 0;

    private int curFocusedIdx = 0;

    List<SwiptBtnScript> buttonObjectPool = new List<SwiptBtnScript>();
    List<SwiptBtnScript> buttonL = new List<SwiptBtnScript>();

    [SerializeField]
    Sprite editerButtonSprite;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR

        
#elif UNITY_ANDROID

#endif

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButton(0))
        {
            curPos = scrollBar.value;

            if (curPos < 0)
            {
                destPos = 0;
                curFocusedIdx = 0;
            }
            else if (curPos > 1)
            {
                destPos = 1;
                curFocusedIdx = buttonL.Count - 1;
            }
            else
            {
                int destIdx = ((int)(curPos / unitDistance));
                destIdx = destIdx % 2 == 0 ? destIdx : destIdx + 1;
                destPos = destIdx * unitDistance;

                curFocusedIdx = destIdx / 2;
            }

        } else {

            scrollBar.value = Mathf.Lerp(scrollBar.value, destPos, 0.1f);

        }

        focusSizeDelegate(Mathf.Clamp(scrollBar.value, 0, 1));
    }


    float unitDistance = 0.5f;

    /// <summary>
    /// 버튼들 초기 정보 세팅.
    /// </summary>
    /// <param name="_stageNameList">노트파일 이름이 저장되어있는 리스트</param>
    public void SettingSwipteButtons(List<string> _stageNameList) {

        int count = _stageNameList.Count + 1;

        if (!CheckObjectPool(count)) ExpandObjectPool(count);

        foreach (SwiptBtnScript _btn in buttonObjectPool) { _btn.gameObject.SetActive(false); }

        SettingButtonsInfo(_stageNameList);

        Recalculate(count);

    }


    /// <summary>
    /// 오브젝트 풀의 갯수 확인
    /// </summary>
    /// <param name="_cnt">필요 갯수 전달</param>
    /// <returns></returns>
    private bool CheckObjectPool(int _cnt) {

        return buttonObjectPool.Count < _cnt ? false : true;

    }

    /// <summary>
    /// 오브젝트 풀의 크기를 확장.
    /// </summary>
    /// <param name="_destSize">목표 확장 크기</param>
    private void ExpandObjectPool(int _destSize) {

        while (buttonObjectPool.Count < _destSize) {
            GameObject tmpBtnObj = Instantiate(ButtonInstance);
            tmpBtnObj.transform.SetParent(ButtonParent);
            buttonObjectPool.Add(tmpBtnObj.GetComponent<SwiptBtnScript>());
        }

    }

    /// <summary>
    /// 버튼들의 내용을 추가하고 사용중 버튼 리스트에 추가
    /// </summary>
    /// <param name="_stageNameList">스테이지들의 이름 배열</param>
    private void SettingButtonsInfo(List<string> _stageNameList) {

        string suffixStr = ".dat";

        buttonL.Clear();

        for(int i = 0; i < _stageNameList.Count + 1; i++) { buttonL.Add(buttonObjectPool[i]); }

        for (int i = 0; i < _stageNameList.Count; i++) {
            buttonL[i].gameObject.SetActive(true);
            buttonL[i].SetText(_stageNameList[i].Substring(0, _stageNameList[i].Length - suffixStr.Length));
        }

        buttonL[buttonL.Count - 1].gameObject.SetActive(true);
        buttonL[buttonL.Count - 1].GetComponent<Image>().sprite = editerButtonSprite;
        buttonL[buttonL.Count - 1].SetText("+");

    }

    /// <summary>
    /// 버튼들의 스와이프/클릭 이벤트등의 재계산 및 재할당
    /// </summary>
    /// <param name="_buttonCount">버튼들의 총 갯수</param>
    private void Recalculate(int _buttonCount) {

        focusSizeDelegate = null;

        if (_buttonCount <= 1) {

            //todo : 예외 처리
            return;
        }

        unitDistance = 1f / ((_buttonCount - 1) * 2);

        for (int i = 0; i < _buttonCount; i++) {
            buttonL[i].InitData(i, unitDistance, ButtonClicked);
            focusSizeDelegate += buttonL[i].SetButtonSize;
        }

    }

    /// <summary>
    /// 버튼이 클릭되었을 때 호출될 콜백 메소드
    /// </summary>
    /// <param name="_btnIdx">받아올 버튼의 고유 idx</param>
    public void ButtonClicked(int _btnIdx) {

        destPos = _btnIdx * unitDistance * 2;

        if (curFocusedIdx == _btnIdx)
        {
            //todo : 장면 전환.
            if (_btnIdx == buttonL.Count - 1)
            {
                Debug.Log("에디터로 전환 &");
                SceneManager.LoadScene("MakeScene", LoadSceneMode.Single);

            }
            else
            {
                Debug.Log("장면 전환 & : " + _btnIdx + buttonL[_btnIdx].GetText());

                //todo : 미리 한번 파일 읽어보고 올바르게 열 수 있는지 확인.

                SceneManager.LoadScene("SampleScene 1", LoadSceneMode.Single);
                PlayerPrefs.SetString("StageFileName", buttonL[_btnIdx].GetText() + ".dat");
            }
        }
        else { 
            curFocusedIdx = _btnIdx;
            Debug.Log("포커스 전환 & to : " + _btnIdx);
        }

    }



}
