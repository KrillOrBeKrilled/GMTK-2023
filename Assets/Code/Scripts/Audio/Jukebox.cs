//*******************************************************************************************
// Jukebox
//*******************************************************************************************
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

        protected override void Awake()
        {
            base.Awake();

            if (!IsLoaded)
            {
                this.PlayMusicEvent.Post(this.gameObject);
                DontDestroyOnLoad(this.gameObject);
            }
            IsLoaded = true;
        }

        public void PlayMusic()
        {
            this.PlayMusicEvent.Post(this.gameObject);
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
    }
}
