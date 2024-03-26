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
    GameObject contentParent;


    FocusSizeDelegate focusSizeDelegate = null;

    float curPos = 0;
    float destPos = 0;

    private int curFocusedIdx = 0;

    List<SwiptBtnScript> buttonL;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        Recalculate(3);
#elif UNITY_ANDROID

#endif

    }

    // Update is called once per frame
    void Update()
    {

        //에디터용 디버그 테스트
        if(Input.GetKeyDown(KeyCode.Space)){

            Recalculate(3);

        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            Recalculate(4);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            Recalculate(5);

        }


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

        }else {

            scrollBar.value = Mathf.Lerp(scrollBar.value, destPos, 0.1f);

        }

        focusSizeDelegate(Mathf.Clamp(scrollBar.value, 0, 1));
    }


    float unitDistance = 0.5f;

    public void Recalculate(int _buttonCount) {

        focusSizeDelegate = null;

        if (_buttonCount <= 1) {

            //todo : 예외 처리
            return;
        }

        buttonL = new List<SwiptBtnScript>();

        for (int i = 0; i < _buttonCount; i++) {
            buttonL.Add(contentParent.transform.GetChild(i).GetComponent<SwiptBtnScript>());
        }

        unitDistance = 1f / ((_buttonCount - 1) * 2);

        for (int i = 0; i < _buttonCount; i++) {
            buttonL[i].gameObject.SetActive(true);
            buttonL[i].InitData(i, unitDistance, ButtonClicked);
            focusSizeDelegate += buttonL[i].SetButtonSize;
        }

    }

    public void ButtonClicked(int _btnIdx) {

        destPos = _btnIdx * unitDistance * 2;

        if (curFocusedIdx == _btnIdx)
        {
            //todo : 장면 전환.
            if (_btnIdx == buttonL.Count - 1)
            {
                Debug.Log("에디터로 전환 &");
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
