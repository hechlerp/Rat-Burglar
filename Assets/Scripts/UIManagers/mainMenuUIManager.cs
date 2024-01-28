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
        SceneManager.LoadScene(1);
        AudioManager.Instance.fmodBgm.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.fmodBgm.release();

        // AudioManager.Instance.StopBGM();

    }


    public void PressQuit() {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    
}
