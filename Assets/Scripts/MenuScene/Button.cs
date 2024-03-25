using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{

    private int myIdx;
    private float unitDistance;

    private float smallSize = 0.8f;

    [SerializeField]
    Text childText;

    public void InitData(int _idx, float _unitDistance) { 
        myIdx = _idx;
        unitDistance = _unitDistance;
    }

    /// <summary>
    /// 0~1 사이의 스크롤바 값이 들어옴
    /// </summary>
    /// <param name="_barValue"></param>
    public void SetButtonSize(float _barValue) { 
        
        float localUnitDistance = Mathf.Abs(Mathf.Clamp(_barValue - (myIdx * 2 * unitDistance), -unitDistance * 2, unitDistance * 2));

        Debug.Log(myIdx + " idx = " + localUnitDistance);

        float localValue = 1 - ((localUnitDistance / (unitDistance * 2)) * (1 - smallSize));

        transform.localScale = new Vector2(localValue, localValue);

    }

    public void SetText(string _text) {
        childText.text = _text;
    }

}
