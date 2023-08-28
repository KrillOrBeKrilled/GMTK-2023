using Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// AudioManager
//*******************************************************************************************
namespace Managers {
    /// <summary>
    /// A class to act as a soundbank for all the game's SFX. Works hand in hand with the
    /// <see cref="Jukebox"> Jukebox </see> class (handles the music soundbank) to provide
    /// methods for listening in on events invoked during gameplay that handle all the Wwise
    /// sound events.
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
            this._jukebox = GameObject.Find("Jukebox")?.GetComponent<Jukebox>();

            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                PauseManager.Instance.OnPauseToggled.AddListener(this.ToggleJukeboxPause);
            }

        }

        //========================================
        // UI Sound Event Methods
        //========================================
        public void PlayUIClick(GameObject audioSource)
        {
            this._playUIConfirmEvent.Post(audioSource);
        }

        public void PlayUIHover(GameObject audioSource)
        {
            this._playUISelectEvent.Post(audioSource);
        }

        public void PlayUITileSelectMove(GameObject audioSource)
        {
            this._playUITileSelectMoveEvent.Post(audioSource);
        }

        public void PlayUITileSelectConfirm(GameObject audioSource)
        {
            this._playUITileSelectConfirmEvent.Post(audioSource);
        }

        private void ToggleJukeboxPause(bool isPaused)
        {
            if (this._jukebox is null) return;

            if (isPaused)
            {
                this._jukebox.PauseMusic();
                this._playUIPauseEvent.Post(this.gameObject);
                return;
            }

            this._playUIUnpauseEvent.Post(this.gameObject);
            this._jukebox.UnpauseMusic();
        }

        //========================================
        // Hen Sound Event Methods
        //========================================
        public void PlayBuild(GameObject audioSource)
        {
            if (!this._isBuilding)
            {
                this._isBuilding = true;
                this.StartCoroutine(this.PlayBuildSoundForDuration(11f));
            }
        }

        public void StopBuild(GameObject audioSource)
        {
            this.StopCoroutine(this.PlayBuildSoundForDuration(11f));
            this._stopBuildEvent.Post(this.gameObject);

            this._isBuilding = false;
        }

        private IEnumerator PlayBuildSoundForDuration(float durationInSeconds)
        {
            while (this._isBuilding)
            {
                this._startBuildEvent.Post(this.gameObject);
                yield return new WaitForSeconds(durationInSeconds);
                this._stopBuildEvent.Post(this.gameObject);
            }
        }

        public void PlayBuildComplete(GameObject audioSource)
        {
            this._buildCompleteEvent.Post(audioSource);
        }

        public void PlayHenDeath(GameObject audioSource)
        {
            this._henDeathEvent.Post(audioSource);
        }

        public void PlayHenJump(GameObject audioSource)
        {
            this._henFlapEvent.Post(audioSource);
        }
    }
}
