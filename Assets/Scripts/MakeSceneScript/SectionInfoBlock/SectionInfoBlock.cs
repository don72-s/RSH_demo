using UnityEngine;
using UnityEngine.UI;

public class SectionInfoBlock : MonoBehaviour {

    Color defaultColor = new Color(1, 1, 1, 0.39215686f);
    Color focusColor = new Color(0.40392157f, 1, 0.30588235f, 0.39215686f);
    private Image myImg;

    [SerializeField]
    Text text;

    private void Awake() {

        myImg = GetComponent<Image>();

    }

    public void SetFocus(bool _onFocus) {

        myImg.color = _onFocus ? focusColor : defaultColor;

    }

    public void SetText(string _str) {

        text.text = _str;

    }

    public void ClearText() {

        SetText("");

    }

}
