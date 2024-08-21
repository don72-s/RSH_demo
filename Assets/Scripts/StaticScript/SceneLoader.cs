using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;

    public void btn_changeScene() {

        Time.timeScale = 1;
        StopAllCoroutines();
        SceneManager.LoadScene(SceneName);

    }
}
