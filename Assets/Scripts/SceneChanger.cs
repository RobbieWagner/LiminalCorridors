using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    // Used to change scenes 

    [SerializeField]
    private string sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);    
    }
}
