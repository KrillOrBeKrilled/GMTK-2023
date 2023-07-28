using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event 
        _playUIConfirmEvent, 
        _playUISelectEvent, 
        _playUITileSelectMoveEvent, 
        _playUITileSelectConfirmEvent;

    private Jukebox _jukebox;

    private void Start()
    {
        _jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            PauseManager.Instance.OnPauseToggled.AddListener(ToggleJukeboxPause);
        }
        
    }

    public void PlayUIClick(GameObject audioSource)
    {
        _playUIConfirmEvent.Post(audioSource);
    }

    public void PlayUIHover(GameObject audioSource)
    {
        _playUISelectEvent.Post(audioSource);
    }
    
    public void PlayUITileSelectMove(GameObject audioSource)
    {
        _playUITileSelectMoveEvent.Post(audioSource);
    }
    
    public void PlayUITileSelectConfirm(GameObject audioSource)
    {
        _playUITileSelectConfirmEvent.Post(audioSource);
    }

    private void ToggleJukeboxPause(bool isPaused)
    {
        if (isPaused)
        {
            _jukebox.PauseMusic();
            return;
        }
        
        _jukebox.PlayMusic();
    }
}