using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteDisplayere : MonoBehaviour {

    [Header("Parents")]
    [SerializeField]
    RectTransform hitLineTrans;
    [SerializeField]
    RectTransform arrowNotesTrans;

    [Header("NoteObj and Sprites")]
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


    int noteIdx = 0;
    List<GameObject> ArrowIconLists;
    List<Image> ArrowIconImgLists;

    Vector3 startPos;
    Vector3 endPos;

    private void Start() {

        ArrowIconLists = new List<GameObject>();
        ArrowIconImgLists = new List<Image>();

        startPos = hitLineTrans.localPosition;
        endPos = -hitLineTrans.localPosition;

        //아이콘 오브젝트 풀 제작[ 기본 갯수는 16개로 지정 ]
        for (int i = 0; i < 16; i++) {

            ArrowIconLists.Add(Instantiate(ArrowIconObject));
            ArrowIconImgLists.Add(ArrowIconLists[i].GetComponent<Image>());
            ArrowIconLists[i].transform.SetParent(arrowNotesTrans);
            ArrowIconLists[i].transform.localScale = Vector3.one;

        }

        ClearDisplayedNotes();

    }

    /// <summary>
    /// 판정라인을 출발시킴
    /// </summary>
    /// <param name="_totalMoveTime">움직일 총 시간</param>
    public void StartMovingMethod(float _totalMoveTime) {

        StopAllCoroutines();
        StartCoroutine(MoveHitLine(_totalMoveTime));

    }

    private IEnumerator MoveHitLine(float _movingTime) {

        float startTime = 0;

        while (true) {

            if (startTime < _movingTime) {

                float timeRatio = startTime / _movingTime;
                hitLineTrans.localPosition = Vector3.Lerp(startPos, endPos, timeRatio);

            } else {

                hitLineTrans.localPosition = endPos;
                yield break;

            }

            startTime += Time.deltaTime;

            yield return null;

        }

    }

    public void DisplayUpperNote() {

        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(hitLineTrans.localPosition.x, 25, 0);
        ArrowIconImgLists[noteIdx].sprite = UpperIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;

    }

    public void DisplayLowerNote() {

        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(hitLineTrans.localPosition.x, -25, 0);
        ArrowIconImgLists[noteIdx].sprite = LowerIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;

    }

    public void DisplayInverseUpperNote() {

        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(hitLineTrans.localPosition.x, 25, 0);
        ArrowIconImgLists[noteIdx].sprite = UpperInverseIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;

    }

    public void DisplayInverseLowerNote() {

        CheckObjectPool();
        ArrowIconLists[noteIdx].transform.localPosition = new Vector3(hitLineTrans.localPosition.x, -25, 0);
        ArrowIconImgLists[noteIdx].sprite = LowerInverseIconImg;
        ArrowIconLists[noteIdx].SetActive(true);
        noteIdx++;

    }

    public void CheckObjectPool() {

        if (noteIdx >= ArrowIconLists.Count) {

            GameObject tmpObj = Instantiate(ArrowIconObject);
            ArrowIconLists.Add(tmpObj);
            ArrowIconImgLists.Add(tmpObj.GetComponent<Image>());
            tmpObj.transform.SetParent(arrowNotesTrans);
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
