using UnityEngine;

//*******************************************************************************************
// Jukebox
//*******************************************************************************************
/// <summary>
/// A class to act as a soundbank for all the game's music. Works hand in hand with the
/// AudioManager class (handles the SFX soundbank) to provide methods for listening in on
/// events invoked during gameplay that handle all the Wwise sound events.
/// </summary>
public class Jukebox : Singleton<Jukebox>
{
    public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;
    
    public static bool IsLoaded;
    
    void Awake()
    {
        base.Awake();
        
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

    public void StopMusic()
    {
        StopMusicEvent.Post(gameObject);
    }
}
