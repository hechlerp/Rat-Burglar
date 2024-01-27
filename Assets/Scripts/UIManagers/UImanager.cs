using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject helpMenu;
    private bool currentHelpMenuCheck = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PressEscape();
        }
    }

    public void PressEscape() {
        Debug.Log("pressed");
        currentHelpMenuCheck = !currentHelpMenuCheck;
        Time.timeScale = currentHelpMenuCheck? 0: 1 ;
        helpMenu.SetActive(currentHelpMenuCheck);
    }
}
