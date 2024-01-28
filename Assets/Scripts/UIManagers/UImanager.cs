using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    // Start is called before the first frame update
    private static UImanager instance;
    public static UImanager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UImanager>();
            }
            return instance;
        }
    }

    public GameObject helpMenu;
    public GameObject WinPanel;
    public GameObject LosePanel;

    private bool currentHelpMenuCheck = false;

    private void Start()
    {
        Time.timeScale = 1;
    }
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

    public void PressRestart() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PressBackToMenu() { 
        SceneManager.LoadScene(0);
    }

    public void ShowWin() { 
        Time.timeScale = 0 ;
        WinPanel.SetActive(true);
        AudioManager.Instance.PlayWin();
    
    }

    public void ShowLose() {
        Time.timeScale = 0;
        LosePanel.SetActive(true);
        AudioManager.Instance.PlayDead();

    }
}
