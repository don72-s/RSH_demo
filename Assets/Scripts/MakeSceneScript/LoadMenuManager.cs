using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour {


    [Header("LoadName InputField")]
    [SerializeField]
    InputField fileNameInputField;
    [SerializeField]
    Text resultText;

    [Header("Other Systems")]
    [SerializeField]
    MusicInfoSetter musicInfoSetter;
    [SerializeField]
    SectionInfoDis sectionInfoDisplayer;



    public void btn_LoadFile() {

        string fileName = fileNameInputField.text;
        StageInfo stageInfo = musicInfoSetter.LoadNoteData(fileName);

        if (stageInfo != null) {

            sectionInfoDisplayer.SetupSectionDisplay(stageInfo);

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

        } else {

            resultText.text = "파일을 찾을수가 없습니다_loadfile.";

        }

    }


}
