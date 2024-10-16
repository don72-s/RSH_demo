﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SectionInfoDis : MonoBehaviour {

    [SerializeField]
    Transform secInfoBlockParent;
    [SerializeField]
    SectionInfoBlock secInfoBlockPrefab;
    List<SectionInfoBlock> secInfoBlockL = new List<SectionInfoBlock>();

    /// <summary>
    /// 구간 정보 출력 전광판 초기화.
    /// </summary>
    /// <param name="_stageInfo">참고할 스테이지 정보 객체</param>
    public void SetupSectionDisplay(StageInfo _stageInfo) {

        if (_stageInfo != null) SetupSectionDisplay(_stageInfo.noteArray);

    }

    /// <summary>
    /// 구간 정보 출력 전광판 초기화.
    /// </summary>
    /// <param name="_noteArr">참고할 노트 배열</param>
    public void SetupSectionDisplay(NoteInfo[] _noteArr) {

        if (_noteArr == null) return;

        StringBuilder sb = new StringBuilder();

        int counter = 0;
        int noteIdx = 0;

        foreach (SectionInfoBlock _block in secInfoBlockL) {

            _block.gameObject.SetActive(false);

        }

        foreach (NoteInfo _info in _noteArr) {

            if (_info.waitScoreCount != 0) {

                if (counter % 2 == 0) {

                    sb.Append("{ ");
                    sb.Append(noteIdx.ToString());
                    sb.Append(" ~ ");

                    noteIdx = noteIdx + (_info.waitScoreCount * 2);
                    sb.Append((noteIdx - 1).ToString());
                    sb.Append(" } [ L : ");
                    sb.Append(_info.waitScoreCount);
                    sb.Append(" ]");

                    while (secInfoBlockL.Count <= counter / 2) {
                        SectionInfoBlock tmpObj = Instantiate(secInfoBlockPrefab);
                        tmpObj.transform.SetParent(secInfoBlockParent);
                        secInfoBlockL.Add(tmpObj);
                    }

                    secInfoBlockL[counter / 2].SetText(sb.ToString());
                    secInfoBlockL[counter / 2].gameObject.transform.localScale = Vector3.one;
                    secInfoBlockL[counter / 2].gameObject.SetActive(true);
                    sb.Clear();

                }

                counter++;

            }

        }
    }


    public void ClearFocus() {

        foreach (SectionInfoBlock _block in secInfoBlockL) {

            _block.SetFocus(false);

        }

    }

    public void SetFocus(int _idx) {

        if (_idx >= secInfoBlockL.Count) return;

        secInfoBlockL[_idx].SetFocus(true);

    }

}
