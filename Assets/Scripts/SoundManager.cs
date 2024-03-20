using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public AudioSource bgmplayer;

    public InputManager inputm;

    public NoteDisplayere nDisplayer;

    public Text t;


    public void Pause()
    {
        StopAllCoroutines();
        nDisplayer.StopAllCoroutines();
        bgmplayer.Stop();
        nDisplayer.ClearDisplayedNotes();
    }

    public void playbgm() {

        nDisplayer.ClearDisplayedNotes();

        StopAllCoroutines();
        nDisplayer.StopAllCoroutines();

        bpmStacker = syncOffset;
        curBpmComparer = int.Parse(editPageField.text) * 32 * bpmUnit;
        bpmIndexer = int.Parse(editPageField.text) * 32;

        bpmStacker += curBpmComparer;

        bgmplayer.time = curBpmComparer;
        bgmplayer.Play();

        StartCoroutine(playBGMReader());
    }

    public List<GameObject> noteButtons;

    public void setArrayPart() {

        int baseIdx = int.Parse(editPageField.text) * 32;

        for (int i = 0; i < 16; i++)
        {

            noteArr[baseIdx + i] = noteButtons[i].GetComponent<noteScr>().myType;

        }

    }

    public void SaveNodeData() { 
        NoteDataManager.SaveData(noteArr);
    }

    public void LoadNodeData() {
        noteArr = NoteDataManager.LoadData();
        Debug.Log("loaded");
    }

    public void androidMapLoad() {
        t.text = Application.persistentDataPath;
        noteArr = NoteDataManager.AndroidMapLoadData();
    }

    public GameObject d;

    public InputField editPageField;

    public void setSyncOffset() {

        syncOffset = float.Parse(inf.text);

    }

    public InputField inf;

    float bpmStacker = 0;
    public float syncOffset = 0;

    float curBpmComparer = 0;

    //const float bpmUnit = 0.53571428f; - base BPM
    const float bpmUnit = 0.1339285714285714f;// 1/4 BPM
    int bpmIndexer = 0;

    private IEnumerator playBGMReader() {


        while (bpmStacker < 150) {

            t.text = " " + bpmIndexer;

            if (curBpmComparer <= bpmStacker) {

                if (bpmIndexer % 16 == 0) {
                    nDisplayer.StartMovingMethod();

                    if (bpmIndexer % 32 == 0) { 
                        nDisplayer.ClearDisplayedNotes();
                    }

                }


                while (curBpmComparer <= bpmStacker)
                {
                    curBpmComparer += bpmUnit;


                    switch (noteArr[bpmIndexer++])
                    {

                        case 0:
                            break;

                        case 1:
                            StartCoroutine(LowewrNote());
                            break;

                        case 2:
                            StartCoroutine(UpperNote());
                            break;

                        case 3:
                            StartCoroutine(InverseLowewrNote());
                            break;

                        case 4:
                            StartCoroutine(InverseUpperNote());
                            break;



                    }

                }


            }

            bpmStacker += Time.deltaTime;

            yield return null;

        }


    }

    IEnumerator LowewrNote() {

        inputm.PlayDown();
        nDisplayer.DisplayLowerNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayDown();

    }

    IEnumerator UpperNote()
    {

        inputm.PlayUp();
        nDisplayer.DisplayUpperNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayUp();

    }


    IEnumerator InverseLowewrNote()
    {

        inputm.PlayDown();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseLowerNote();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayDown();

    }

    IEnumerator InverseUpperNote()
    {

        inputm.PlayUp();
        Handheld.Vibrate();
        nDisplayer.DisplayInverseUpperNote ();

        yield return new WaitForSeconds(bpmUnit * 16);

        //inputm.PlayUp();

    }



    int[] noteArr = {

                     
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,

                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0,
                     0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0


    };

}
