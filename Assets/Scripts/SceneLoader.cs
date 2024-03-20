using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;

    public void btn_changeScene() { 
    
        StopAllCoroutines();
        SceneManager.LoadScene(SceneName);

    }
}
