//*******************************************************************************************
// Jukebox
//*******************************************************************************************
using Managers;

namespace Audio {
    /// <summary>
    /// A class to act as a soundbank for all the game's music. Works hand in hand with the
    /// AudioManager class (handles the SFX soundbank) to provide methods for listening in on
    /// events invoked during gameplay that handle all the Wwise sound events.
    /// </summary>
    public class Jukebox : Singleton<Jukebox>
    {
        public AK.Wwise.Event PlayMusicEvent, PauseMusicEvent, UnpauseMusicEvent, StopMusicEvent;

        public static bool IsLoaded;

        private bool _isMusicMuted;

        protected override void Awake()
        {
            base.Awake();

            if (!IsLoaded)
            {
                DontDestroyOnLoad(this.gameObject);
                this._isMusicMuted = PlayerPrefsManager.IsMusicMuted();
                this.PlayMusic();
            }
            IsLoaded = true;
        }

        public void PlayMusic()
        {
            if (!this._isMusicMuted) {
                this.PlayMusicEvent.Post(this.gameObject);
            }
        }

        public void PauseMusic()
        {
            this.PauseMusicEvent.Post(this.gameObject);
        }

        public void UnpauseMusic()
        {
            this.UnpauseMusicEvent.Post(this.gameObject);
        }

        public void StopMusic()
        {
            this.StopMusicEvent.Post(this.gameObject);
        }

        public void SetIsMusicMuted(bool isMuted) {
            this._isMusicMuted = isMuted;
        }
    }
}
