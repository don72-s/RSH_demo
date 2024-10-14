using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : MonoBehaviour {

    public delegate void AlertButtonCallback();

    private AlertButtonCallback callback = null;

    [SerializeField]
    TextMeshProUGUI tmpText;

    [SerializeField]
    GameObject singleButtonGroup;
    [SerializeField]
    GameObject doubleButtonGroup;
    [SerializeField]
    Text optionalButtonText;


    public void SetAlertMessage(in string _message) {
        tmpText.text = _message;
    }

    public void CloseAlertWindow() {
        gameObject.SetActive(false);
    }

    private void DisableAllButtonGroup() {
        singleButtonGroup.SetActive(false);
        doubleButtonGroup.SetActive(false);
    }

    public void ShowSingleAlertWindow(in string _message) {
        SetAlertMessage(_message);
        DisableAllButtonGroup();
        gameObject.SetActive(true);
        singleButtonGroup.SetActive(true);
    }

    public void ShowDoubleAlertWindow(in string _message, in string _optionalBtnText, AlertButtonCallback _callback) {
        SetAlertMessage(_message);

        DisableAllButtonGroup();
        doubleButtonGroup.SetActive(true);
        optionalButtonText.text = _optionalBtnText;

        SetCallbackMethod(_callback);

        gameObject.SetActive(true);
    }

    public void btn_OptionalBtn() {
        callback?.Invoke();
    }

    private void SetCallbackMethod(AlertButtonCallback _callbackMethod) {
        callback = _callbackMethod;
    }

}
