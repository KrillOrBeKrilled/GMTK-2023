//*******************************************************************************************
// Jukebox
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Acts as a sound bank for all the game's music. Works hand in hand with the
    /// AudioManager (handles the SFX soundbank) to provide methods for listening in on
    /// events invoked during gameplay that handle all the Wwise sound events.
    /// </summary>
    /// <remarks> The GameObject associated with this class will persist through scene loads. </remarks>
    public class Jukebox : Singleton<Jukebox> {
        public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;

        private static bool _isLoaded;

        private new void Awake() {
            base.Awake();

            if (!_isLoaded) {
                this.PlayMusicEvent.Post(this.gameObject);
                DontDestroyOnLoad(this.gameObject);
            }
            _isLoaded = true;
        }

        /// <summary>
        /// Plays the main game music.
        /// </summary>
        public void PlayMusic() {
            this.PlayMusicEvent.Post(this.gameObject);
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
    }
}
