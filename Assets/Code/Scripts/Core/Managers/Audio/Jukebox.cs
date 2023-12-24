using UnityEngine;

//*******************************************************************************************
// Jukebox
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers.Audio {
    /// <summary>
    /// Acts as a sound bank for all the game's music. Works hand in hand with the
    /// AudioManager (handles the SFX soundbank) to provide methods for listening in on
    /// events invoked during gameplay that handle all the Wwise sound events.
    /// </summary>
    /// <remarks> The GameObject associated with this class will persist through scene loads. </remarks>
    public class Jukebox : Singleton<Jukebox> {
        [SerializeField] private AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;

        public static bool IsLoaded;
        private bool _isMusicMuted;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private new void Awake() {
            base.Awake();

            if (!IsLoaded) {
                DontDestroyOnLoad(this.gameObject);
                this._isMusicMuted = PlayerPrefsManager.IsMusicMuted();
                this.PlayMusic();
            }
            IsLoaded = true;
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Plays the main game music.
        /// </summary>
        public void PlayMusic() {
            if (!this._isMusicMuted) {
                this.PlayMusicEvent.Post(this.gameObject);
            }
        }
        
        /// <summary>
        /// Stops the main game music.
        /// </summary>
        public void StopMusic() {
            this.StopMusicEvent.Post(this.gameObject);
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Pauses the main game music.
        /// </summary>
        internal void PauseMusic() {
            this.PauseMusicEvent.Post(this.gameObject);
        }
        
        /// <summary>
        /// Toggles the setting to mute the game music, muting the game music.
        /// </summary>
        /// <param name="isMuted"> If the game music is muted. </param>
        /// <remarks>
        /// Set in the settings/pause menu, which will invoke <see cref="PlayMusic"/> when reentering the game.
        /// </remarks>
        internal void SetIsMusicMuted(bool isMuted) {
            this._isMusicMuted = isMuted;
        }
        
        /// <summary>
        /// Plays the main game music, leaving off from when it was paused.
        /// </summary>
        internal void UnpauseMusic() {
            this.UnpauseMusicEvent.Post(this.gameObject);
        }
        
        #endregion
    }
}
