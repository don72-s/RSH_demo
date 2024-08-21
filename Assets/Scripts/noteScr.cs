using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class noteScr : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (myType == 0)
            {
                myImg.color = new Color(mycolor.r, mycolor.g, mycolor.b, 0.1f);
                myType = 1;
            }
            else if (myType == 1)
            {

                myImg.color = new Color(mycolor.r, mycolor.g, mycolor.b, 0.5f);
                myType = 2;

            }
            else if (myType == 2)
            {
                myImg.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.1f);
                myType = 3;
            }
            else if (myType == 3)
            {
                myImg.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
                myType = 4;
            }
            else {

                myImg.color = new Color(mycolor.r, mycolor.g, mycolor.b, 1f);
                myType = 0;
            }


        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            myImg.color = new Color(mycolor.r, mycolor.g, mycolor.b, 1f);
            myType = 0;

        }
    }


    private Image myImg;
    public int myType = 0;
    private Color mycolor;

    // Start is called before the first frame update
    void Start()
    {
        myImg = GetComponent<Image>();
        mycolor = myImg.color;
    }


}
