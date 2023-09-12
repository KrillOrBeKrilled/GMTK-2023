//*******************************************************************************************
// Jukebox
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers.Audio {
    /// <summary>
    /// Acts as a sound bank for all the game's music. Works hand in hand with the
    /// AudioManager (handles the SFX soundbank) to provide methods for listening in on
    /// events invoked during gameplay that handle all the Wwise sound events.
    /// </summary>
    /// <remarks> The GameObject associated with this class will persist through scene loads. </remarks>
    public class Jukebox : Singleton<Jukebox> {
        public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;

        public static bool IsLoaded;
        private bool _isMusicMuted;

        private new void Awake() {
            base.Awake();

            if (!IsLoaded) {
                DontDestroyOnLoad(this.gameObject);
                this._isMusicMuted = PlayerPrefsManager.IsMusicMuted();
                this.PlayMusic();
            }
            IsLoaded = true;
        }

        /// <summary>
        /// Plays the main game music.
        /// </summary>
        public void PlayMusic() {
            if (!this._isMusicMuted) {
                this.PlayMusicEvent.Post(this.gameObject);
            }
        }

        /// <summary>
        /// Pauses the main game music.
        /// </summary>
        public void PauseMusic() {
            this.PauseMusicEvent.Post(this.gameObject);
        }

        /// <summary>
        /// Plays the main game music, leaving off from when it was paused.
        /// </summary>
        public void UnpauseMusic() {
            this.UnpauseMusicEvent.Post(this.gameObject);
        }

        /// <summary>
        /// Stops the main game music.
        /// </summary>
        public void StopMusic() {
            this.StopMusicEvent.Post(this.gameObject);
        }
        
        public void SetIsMusicMuted(bool isMuted) {
            this._isMusicMuted = isMuted;
        }
    }
}
