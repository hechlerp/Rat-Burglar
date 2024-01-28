using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{

    private string fmodBgmEvent = "event:/bgm";
    public FMOD.Studio.EventInstance fmodBgm;

    private string fmodAmbienceEvent = "event:/ambience";
    private FMOD.Studio.EventInstance fmodAmbience;

    private string fmodRat1WalkEvent = "event:/rat2Walk";
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

    public GameObject walk1;
    public GameObject walk2;
    public GameObject drag;

    private int countwalk1;
    private bool test = false;
    public bool stop = false;





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


    // Update is called once per frame
    private void Start()
    {
        /*
        fmodRat2Walk = FMODUnity.RuntimeManager.CreateInstance(fmodRat2WalkEvent);
        fmodRatDrag = FMODUnity.RuntimeManager.CreateInstance(fmodRatDragEvent);
        countwalk1 = 0;
        */
        stop = false;

    }
    private void PlaySound(FMOD.Studio.EventInstance i, string fmodevent) {
        if (i.isValid())
        {
            // Debug.Log("is valid");
        }
        else
        {
            i = FMODUnity.RuntimeManager.CreateInstance(fmodevent);
            i.start();
            i.release();
            // Debug.Log("change");
        }
    }

    public void StopSound(FMOD.Studio.EventInstance i) {
        i.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        i.release();
    }

    public void PlayBGM() {
        /*
        FMODUnity.RuntimeManager.PlayOneShot(fmodBgmEvent);
        PlaySound(fmodBgm, fmodBgmEvent);
        */

        fmodBgm= FMODUnity.RuntimeManager.CreateInstance(fmodBgmEvent);
        fmodBgm.start();
    }
    public void StopBGM()
    {
        StopSound(fmodBgm);
    }
    public void PlayAmbience()
    {

        PlaySound(fmodAmbience, fmodAmbienceEvent);
    }

    public void PlayRat1Walk()
    {
        if (!stop) {
            FMODUnity.RuntimeManager.PlayOneShot(fmodRat1WalkEvent);
        }

        /*
        fmodRat1Walk = FMODUnity.RuntimeManager.CreateInstance(fmodRat2WalkEvent);
        fmodRat1Walk.start();
        */
        /*
        if (countwalk1 == 20)
        {
            countwalk1 = 0;
        }
        if (countwalk1 == 0)
            FMODUnity.RuntimeManager.PlayOneShot(fmodRat1WalkEvent);
        countwalk1++;
        */
        /*
        if (check)
        {
            if (countwalk1 == 0)
                Debug.Log("whats");
            FMODUnity.RuntimeManager.PlayOneShot(fmodRat1WalkEvent);
            if (countwalk1 == 100)
            {
                countwalk1 = 0;
            }
            countwalk1 += 1;
            Debug.Log(countwalk1);


        }
                    */
    }

    public void StopRat1Walk()
    {
        if (fmodRat1Walk.isValid()) {
            fmodRat1Walk.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            fmodRat1Walk.release();
        }
    }

    public void PlayRat2Walk(bool check)
    {
        walk2.SetActive(check);
    }
    /*
    public void StopRat2Walk()
    {
        fmodRat2Walk.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    */

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
        if (!stop)
        {
            FMODUnity.RuntimeManager.PlayOneShot(fmodRatDragEvent);
        }
    }

    public void StopRatDrag()
    {
        fmodRatDrag.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
