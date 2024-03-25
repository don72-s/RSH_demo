using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    List<Button> buttonL;

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

        } else if (Input.GetMouseButtonUp(0)) {

            if (curPos < 0)
            {
                destPos = 0;
            }
            else if (curPos > 1)
            {
                destPos = 1;
            }
            else
            {
                int destIdx = ((int)(curPos / unitDistance));
                destIdx = destIdx % 2 == 0 ? destIdx : destIdx + 1;
                destPos = destIdx * unitDistance;
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

        buttonL = new List<Button>();

        for (int i = 0; i < contentParent.transform.childCount; i++) { 
            buttonL.Add(contentParent.transform.GetChild(i).GetComponent<Button>());
        }

        unitDistance = 1f / ((_buttonCount - 1) * 2);

        for (int i = 0; i < _buttonCount; i++) {
            buttonL[i].gameObject.SetActive(true);
            buttonL[i].InitData(i, unitDistance);
            focusSizeDelegate += buttonL[i].SetButtonSize;
        }

    }


}
