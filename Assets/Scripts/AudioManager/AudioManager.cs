using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private string fmodBgmEvent = "event:/bgm";
    private FMOD.Studio.EventInstance fmodBgm;

    private string fmodAmbienceEvent = "event:/ambience";
    private FMOD.Studio.EventInstance fmodAmbience;

    private string fmodRat1WalkEvent = "event:/rat1Walk";
    private FMOD.Studio.EventInstance fmodRat1Walk;

    private string fmodRat2WalkEvent = "event:/rat2Walk";
    private FMOD.Studio.EventInstance fmodRat2Walk;

    private string fmodRat1SqueakEvent = "event:/rat1Squeak";
    private FMOD.Studio.EventInstance fmodRat1Squeak;

    private string fmodRat2SqueakEvent = "event:/rat2Walk";
    private FMOD.Studio.EventInstance fmodRat2Squeak;

    private string fmodRatDragEvent = "event:/ratDrag";
    private FMOD.Studio.EventInstance fmodRatDrag;

    private string fmodRatWinEvent = "event:/ratWin";
    private FMOD.Studio.EventInstance fmodRatWin;

    private string fmodRatDeadEvent = "event:/ratDead";
    private FMOD.Studio.EventInstance fmodRatDead;


    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }


        }
    
    void Awake()

    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame

    public void PlaySound(FMOD.Studio.EventInstance i, string fmodevent) {
        if (i.isValid())
        {
            // Debug.Log("is valid");
        }
        else
        {
            i = FMODUnity.RuntimeManager.CreateInstance(fmodevent);
            i.start();
            // Debug.Log("change");
        }
    }

    public void StopSound(FMOD.Studio.EventInstance i) {
        i.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        i.release();
    }
    public void PlayBGM() {
        PlaySound(fmodBgm, fmodBgmEvent);
    }

    public void PlayAmbience()
    {
        PlaySound(fmodAmbience, fmodAmbienceEvent);
    }

    public void PlayRat1Walk()
    {
        PlaySound(fmodRat1Walk, fmodRat1WalkEvent);
    }

    public void StopRat1Walk()
    {
        StopSound(fmodRat1Walk);
    }

    public void PlayRat2Walk()
    {
        PlaySound(fmodRat2Walk, fmodRat2WalkEvent);
    }

    public void StopRat2Walk()
    {
        StopSound(fmodRat2Walk);
    }

    public void PlayRat1Squeak()
    {
        PlaySound(fmodRat1Squeak, fmodRat1SqueakEvent);
    }

    public void StopRat1Squeak()
    {
        StopSound(fmodRat1Squeak);
    }

    public void PlayRat2Squeak()
    {
        PlaySound(fmodRat2Squeak, fmodRat2SqueakEvent);
    }

    public void StopRat2Squeak()
    {
        StopSound(fmodRat2Squeak);
    }

    public void PlayRatDrag()
    {
        PlaySound(fmodRatDrag, fmodRatDragEvent);
    }

    public void StopDrag()
    {
        StopSound(fmodRatDrag);
    }

    public void PlayWin()
    {
        PlaySound(fmodRatWin, fmodRatWinEvent);
    }

    public void StopWin()
    {
        StopSound(fmodRatWin);
    }

    public void PlayDead()
    {
        PlaySound(fmodRatDead, fmodRatDeadEvent);
    }

    public void StopDead()
    {
        StopSound(fmodRatDead);
    }
}
