using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{

    [SerializeField]
    private bool timedQuit = false;

    // Allows the user to quit the game
    public void QuitApplication()
    {
        Application.Quit();
    }

    void Start()
    {
        if(timedQuit) StartCoroutine(WaitThenQuit());
    }

    void Update() 
    {
        if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Q)) QuitApplication();
    }

    IEnumerator WaitThenQuit()
    {
        yield return new WaitForSeconds(4f);
        QuitApplication();
    }
}
