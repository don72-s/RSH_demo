using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteDisplayere : MonoBehaviour
{

    public GameObject ArrowGroup;
    public GameObject ArrowUpperHead;
    public GameObject ArrowLowerHead;

    RectTransform rect;
    Vector3 startPos;
    Vector3 endPos;
    float movingTime;

    private float startArrowPosX;
    private void Start()
    {
        rect = ArrowGroup.GetComponent<RectTransform>();

        startPos = rect.localPosition;
        endPos = -rect.localPosition;
        movingTime =  0.53571428f * 4;

        defaultColor = displayNoteLists[0].GetComponent<Image>().color;

    }

    private float startTime;

    public void StartMovingMethod() { 

        StopAllCoroutines();
        SwapArrow();
        StartCoroutine(moveArrow(movingTime));

    }

    public void StartMovingMethod(float _totalMoveTime)
    {

        StopAllCoroutines();
        SwapArrow();
        StartCoroutine(moveArrow(_totalMoveTime));

    }

    private void SwapArrow() {

        ArrowUpperHead.SetActive(!ArrowUpperHead.activeSelf);
        ArrowLowerHead.SetActive(!ArrowLowerHead.activeSelf);

    }

    private IEnumerator moveArrow(float _allTime) { 
    
        startTime = Time.time;

        while (true) { 
        
            float curDurTime = Time.time - startTime;

            if (curDurTime < _allTime)
            {

                float timeRatio = curDurTime / _allTime;
                rect.localPosition = Vector3.Lerp(startPos, endPos, timeRatio);

            }
            else {
                rect.localPosition = endPos;
                yield break;
            }

            yield return null;

        }

    }


    private int noteIdx = 0;
    public List<GameObject> displayNoteLists;
    public List<Image> displayImgLists;

    public void DisplayUpperNote() {
        displayNoteLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 107, 0);
        displayImgLists[noteIdx].color = defaultColor;
        displayNoteLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayLowerNote()
    {
        displayNoteLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 74, 0);
        displayImgLists[noteIdx].color = defaultColor;
        displayNoteLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayInverseUpperNote()
    {
        displayNoteLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 107, 0);
        displayImgLists[noteIdx].color = Color.red;
        displayNoteLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayInverseLowerNote()
    {
        displayNoteLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 74, 0);
        displayImgLists[noteIdx].color = Color.red;
        displayNoteLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void ClearDisplayedNotes() {
        foreach (GameObject note in displayNoteLists) { 
            note.SetActive(false);
        }
        noteIdx = 0;
    }

    private Color defaultColor;

}
