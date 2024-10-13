using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{

    private List<InfoPageSO.InfoBlock> infoBlockL = null;

    [SerializeField]
    Text titleText;
    [SerializeField]
    Image image;
    [SerializeField]
    Button leftButton;
    [SerializeField]
    Button rightButton;


    private int curIdx = 0;

    public void Btn_LeftButton() {

        if (curIdx == 0) return;
        curIdx--;
        RefreshPage();

    }

    public void Btn_RightButton() {

        if (curIdx == infoBlockL.Count - 1) return;
        curIdx++;
        RefreshPage();

    }


    public void Btn_InitInfo(InfoPageSO _pageSO) {

        if (_pageSO == null) return;

        if (_pageSO.InfoBlockL == null || _pageSO.InfoBlockL.Count == 0) return;

        gameObject.SetActive(true);

        infoBlockL = _pageSO.InfoBlockL;

        curIdx = 0;
        RefreshPage();

    }


    private void RefreshPage() {

        if (curIdx < 0 || curIdx >= this.infoBlockL.Count) { Debug.LogError("입력 매개변수 범위 초과."); return; }

        if (curIdx == 0)
        {
            leftButton.gameObject.SetActive(false);
        }
        else {
            leftButton.gameObject.SetActive(true);
        }


        if (curIdx == infoBlockL.Count - 1)
        {
            rightButton.gameObject.SetActive(false);
        }
        else {
            rightButton.gameObject.SetActive(true);
        }


        InfoPageSO.InfoBlock tmpBlock = infoBlockL[curIdx];

        titleText.text = tmpBlock.titleText;
        image.sprite = tmpBlock.img;

    }

}
