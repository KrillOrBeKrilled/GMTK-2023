using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AK.Wwise.Event PlayMusicEvent;

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
}
