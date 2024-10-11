using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject loadUIObject;


    [SerializeField]
    InputField fileNameInputField;
    [SerializeField]
    Text resultText;


    [SerializeField]
    MusicInfoSetter musicInfoSetter;

    [SerializeField]
    SectionInfoDis sectionInfoDisplayer;

    public void btn_Active_Load_UI()
    {
        loadUIObject.SetActive(true);
    }

    public void btn_UnActive_Load_UI()
    {
        loadUIObject.SetActive(false);
    }



    public void btn_LoadFile() {

        string fileName = fileNameInputField.text;
        StageInfo stageInfo = musicInfoSetter.LoadNoteData(fileName);

        if (stageInfo != null) {

            sectionInfoDisplayer.SetupSecInfoDisplay(stageInfo);

            StringBuilder sb = new StringBuilder();

            sb.Append("Load Success! \n");
            sb.Append("Stage Name : ");
            sb.Append(fileName.Substring(0, fileName.Length - ".dat".Length));
            sb.Append("\nBGM : ");
            sb.Append(stageInfo.bgmType);
            sb.Append("\nbgm Multiplyer : ");
            sb.Append(stageInfo.bpmMultiplier);
            sb.Append("\nscore Unit : ");
            sb.Append(stageInfo.scoreUnit);
            sb.Append("\n");


            resultText.text = sb.ToString();

        }
        else{
            resultText.text = "파일을 찾을수가 없습니다_loadfile.";
        }

    }


}
