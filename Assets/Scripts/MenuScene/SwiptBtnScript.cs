﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwiptBtnScript : MonoBehaviour
{
    public delegate void SwiptBtnClick(int ownIdx);


    private int myIdx;
    private float unitDistance;

    private float smallSize = 0.8f;

    public SwiptBtnClick myBtnClick = null;

    [SerializeField]
    Text childText;

    public void InitData(int _idx, float _unitDistance, SwiptBtnClick _btnClickedCallback)
    {
        myIdx = _idx;
        unitDistance = _unitDistance;
        myBtnClick = _btnClickedCallback;

    }

    /// <summary>
    /// 0~1 사이의 스크롤바 값이 들어옴
    /// </summary>
    /// <param name="_barValue"></param>
    public void SetButtonSize(float _barValue)
    {

        float localUnitDistance = Mathf.Abs(Mathf.Clamp(_barValue - (myIdx * 2 * unitDistance), -unitDistance * 2, unitDistance * 2));

        float localValue = 1 - ((localUnitDistance / (unitDistance * 2)) * (1 - smallSize));

        transform.localScale = new Vector2(localValue, localValue);

    }

    public void SetText(string _text)
    {
        childText.text = _text;
    }

    public string GetText() { 
        return childText.text;
    }

    public void btn_Cilcked()
    {
        if (myBtnClick != null) {
            myBtnClick(myIdx);
        }
    }
}
