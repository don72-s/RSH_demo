using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SectionInfoDis : MonoBehaviour
{

    [SerializeField]
    Transform secInfoBlockParent;
    [SerializeField]
    GameObject secInfoBlockInstance;
    List<SectionInfoBlock> secInfoBlockL = new List<SectionInfoBlock>();


    public void SetupSecInfoDisplay(StageInfo _stageInfo)
    {
        if (_stageInfo != null) SetupSecInfoDisplay(_stageInfo.noteArray);
            }

    public void SetupSecInfoDisplay(NoteInfo[] _noteArr) {

        if (_noteArr == null) return;

        StringBuilder sb = new StringBuilder();

        int counter = 0;
        int noteIdx = 0;

        foreach (NoteInfo _info in _noteArr)
        {
            Debug.Log(_info.waitScoreCount);
            if (_info.waitScoreCount != 0)
            {

                if (counter % 2 == 0)
                {

                    sb.Append("{ ");
                    sb.Append(noteIdx.ToString());
                    sb.Append(" ~ ");

                    noteIdx = noteIdx + (_info.waitScoreCount * 2);
                    sb.Append((noteIdx - 1).ToString());
                    sb.Append(" }");

                    while (secInfoBlockL.Count <= counter / 2)
                    {
                        GameObject tmpObj = Instantiate(secInfoBlockInstance);
                        tmpObj.transform.SetParent(secInfoBlockParent);
                        secInfoBlockL.Add(tmpObj.GetComponent<SectionInfoBlock>());
                    }

                    secInfoBlockL[counter / 2].SetText(sb.ToString());
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
