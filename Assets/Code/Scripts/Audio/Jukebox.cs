using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, StopMusicEvent;
    
    public static bool IsLoaded;
    
    void Awake()
    {
        if (!IsLoaded)
        {
            PlayMusicEvent.Post(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        IsLoaded = true;
    }

    public void PlayMusic()
    {
        PlayMusicEvent.Post(gameObject);
    }
    
    // TODO: Fix this to pause the game music instead of stopping it
    public void PauseMusic()
    {
        StopMusicEvent.Post(gameObject);
    }
}
