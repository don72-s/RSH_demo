using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteDisplayere : MonoBehaviour
{

    public GameObject ArrowGroup;

    [SerializeField]
    GameObject ArrowIconParent;


    [SerializeField]
    GameObject ArrowIconObject;
    [SerializeField]
    Sprite UpperIconImg;
    [SerializeField]
    Sprite LowerIconImg;
    [SerializeField]
    Sprite UpperInverseIconImg;
    [SerializeField]
    Sprite LowerInverseIconImg;

    private int noteIdx = 0;
    List<GameObject> ArrowIconLists;
    List<Image> ArrowIconImgLists;


    RectTransform rect;
    Vector3 startPos;
    Vector3 endPos;

    private void Start()
    {

        ArrowIconLists = new List<GameObject>();
        ArrowIconImgLists = new List<Image>();


        rect = ArrowGroup.GetComponent<RectTransform>();

        startPos = rect.localPosition;
        endPos = -rect.localPosition;

        //defaultColor = ArrowIconLists[0].GetComponent<Image>().color;

        for (int i = 0; i < 16; i++)
        {
            ArrowIconLists.Add(Instantiate(ArrowIconObject));
            ArrowIconImgLists.Add(ArrowIconLists[i].GetComponent<Image>());
            ArrowIconLists[i].transform.SetParent(ArrowIconParent.transform);
            ArrowIconLists[i].transform.localScale = Vector3.one;
        }

        ClearDisplayedNotes();
    }

    public void StartMovingMethod(float _totalMoveTime)
    {

        StopAllCoroutines();
        StartCoroutine(moveArrow(_totalMoveTime));

    }


    private bool isPause;

    public void setPause(bool _pause) { 
        isPause = _pause;
    }

    private IEnumerator moveArrow(float _movingTime) { 
    

        float startTime = 0;

        while (true) { 
        
            //float curDurTime = Time.time - startTime;

            if (startTime < _movingTime)
            {

                float timeRatio = startTime / _movingTime;
                rect.localPosition = Vector3.Lerp(startPos, endPos, timeRatio);

            }
            else {
                rect.localPosition = endPos;
                yield break;
            }

            if(!isPause) startTime += Time.deltaTime;

            yield return null;

        }

    }

    public void DisplayUpperNote() {
        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 25, 0);
        ArrowIconImgLists[noteIdx].sprite = UpperIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayLowerNote()
    {
        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, -25, 0);
        ArrowIconImgLists[noteIdx].sprite = LowerIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayInverseUpperNote()
    {
        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, 25, 0);
        ArrowIconImgLists[noteIdx].sprite = UpperInverseIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void DisplayInverseLowerNote()
    {
        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(rect.localPosition.x, -25, 0);
        ArrowIconImgLists[noteIdx].sprite = LowerInverseIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;
    }

    public void CheckObjectPool() {

        if (noteIdx >= ArrowIconLists.Count) {

            GameObject tmpObj = Instantiate(ArrowIconObject);
            ArrowIconLists.Add(tmpObj);
            ArrowIconImgLists.Add(tmpObj.GetComponent<Image>());
            tmpObj.transform.SetParent(ArrowIconParent.transform);
            tmpObj.transform.localScale = Vector3.one;
        }

    }

    public void ClearDisplayedNotes() {
        foreach (GameObject note in ArrowIconLists) { 
            note.SetActive(false);
        }
        noteIdx = 0;
    }

}
