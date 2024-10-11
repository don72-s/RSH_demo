using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertWindowStatic : MonoBehaviour
{

    static AlertWindowStatic instance = null;

    [SerializeField]
    AlertWindow alertWindowPrefab;

    AlertWindow alertWindow;

    private void Awake() {

        if (instance == null) {
            instance = this;
            alertWindow = Instantiate(alertWindowPrefab);
            alertWindow.gameObject.SetActive(false);
            alertWindow.transform.SetParent(transform);
            alertWindow.GetComponent<RectTransform>().offsetMin = Vector3.zero;
            alertWindow.GetComponent<RectTransform>().offsetMax = Vector3.zero;
        } else { 
            Destroy(gameObject);
        }

    }

    public static AlertWindowStatic getInstance() { 
        return instance;
    }

    public AlertWindow GetAlertWindow() {

        if (alertWindow != null) { 
            return alertWindow;
        }

        return null;

    }



}
