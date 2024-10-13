using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    public void Btn_ChangeScene() {

        Time.timeScale = 1;
        StopAllCoroutines();
        SceneManager.LoadScene(sceneName);

    }

    public void Btn_ChangeScene(string _destSceneName) {

        Time.timeScale = 1;
        StopAllCoroutines();
        SceneManager.LoadScene(_destSceneName);

    }


}
