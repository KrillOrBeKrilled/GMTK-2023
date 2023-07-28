using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;
    
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
    
    public void PauseMusic()
    {
        PauseMusicEvent.Post(gameObject);
    }
    
    public void UnpauseMusic()
    {
        UnpauseMusicEvent.Post(gameObject);
    }
}
