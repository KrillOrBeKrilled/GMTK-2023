using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// AudioManager
//*******************************************************************************************
/// <summary>
/// A class to act as a soundbank for all the game's SFX. Works hand in hand with the
/// Jukebox class (handles the music soundbank) to provide methods for listening in on
/// events invoked during gameplay that handle all the Wwise sound events.
/// </summary>
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
        _jukebox = GameObject.Find("Jukebox")?.GetComponent<Jukebox>();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            PauseManager.Instance.OnPauseToggled.AddListener(ToggleJukeboxPause);
        }
        
    }

    //========================================
    // UI Sound Event Methods
    //========================================
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
            _playUIPauseEvent.Post(gameObject);
            return;
        }
        
        _playUIUnpauseEvent.Post(gameObject);
        _jukebox.UnpauseMusic();
    }
    
    //========================================
    // Hen Sound Event Methods
    //========================================
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
}