using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    bool pressingKey;
    bool menuOpen;
    [SerializeField] Canvas pauseMenu;

    private void Start() {
        pressingKey = false;
        menuOpen = false;
        pauseMenu.enabled = false;
    }
    // Update is called once per frame
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && !pressingKey) {
            pressingKey = true;
            if(!menuOpen){
                menuOpen = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.enabled = true;
                Time.timeScale = 0;
            }
            else {
                menuOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.enabled = false;
                Time.timeScale = 1;
            }
        }

        if(Input.GetKeyUp(KeyCode.Escape)) {
            pressingKey = false;
        }
    }
}
