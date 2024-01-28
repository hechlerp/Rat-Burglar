using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuUIManager : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        AudioManager.Instance.PlayBGM();
    }
    public void PressStart() {
        // SceneManager.LoadScene();
    }


    public void PressQuit() {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
