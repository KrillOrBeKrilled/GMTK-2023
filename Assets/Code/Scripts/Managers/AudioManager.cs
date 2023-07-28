using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // ------------ UI Sound Effects -------------
    [SerializeField] private AK.Wwise.Event 
        _playUIConfirmEvent, 
        _playUISelectEvent, 
        _playUITileSelectMoveEvent, 
        _playUITileSelectConfirmEvent,
        _playUIPauseEvent, 
        _playUIUnpauseEvent;
    
    // ----------- Hen Sound Effects -------------
    [SerializeField] private AK.Wwise.Event 
        _startBuildEvent, 
        _stopBuildEvent,
        _buildCompleteEvent,
        _henDeathEvent, 
        _henFlapEvent;

    private bool _isBuilding;

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
    
    public void PlayBuild(GameObject audioSource)
    {
        if (!_isBuilding)
        {
            _isBuilding = true;
            StartCoroutine(PlayBuildSoundForDuration(11f));
        }
    }
    
    public void StopBuild(GameObject audioSource)
    {
        StopCoroutine(PlayBuildSoundForDuration(11f));
        _stopBuildEvent.Post(gameObject);

        _isBuilding = false;
    }
    
    private IEnumerator PlayBuildSoundForDuration(float durationInSeconds)
    {
        while (_isBuilding)
        {
            _startBuildEvent.Post(gameObject);
            yield return new WaitForSeconds(durationInSeconds);
            _stopBuildEvent.Post(gameObject);
        }
    }

    public void PlayBuildComplete(GameObject audioSource)
    {
        _buildCompleteEvent.Post(audioSource);
    }
    
    public void PlayHenDeath(GameObject audioSource)
    {
        _henDeathEvent.Post(audioSource);
    }
    
    public void PlayHenJump(GameObject audioSource)
    {
        _henFlapEvent.Post(audioSource);
    }

    private void ToggleJukeboxPause(bool isPaused)
    {
        if (isPaused)
        {
            _jukebox.PauseMusic();
            _playUIPauseEvent.Post(gameObject);
            return;
        }
        
        _jukebox.UnpauseMusic();
        _playUIUnpauseEvent.Post(gameObject);
    }
}