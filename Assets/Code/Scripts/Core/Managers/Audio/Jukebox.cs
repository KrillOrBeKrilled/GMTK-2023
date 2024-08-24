using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [Serializable]
        public struct SceneEvents {
            public JukeboxSceneType Type;
            public AK.Wwise.Event PlayEvent;
            public AK.Wwise.Event PauseEvent;
            public AK.Wwise.Event UnpauseEvent;
            public AK.Wwise.Event StopEvent;
        }

        public enum JukeboxSceneType {
            Lobby,
            Game
        }
        
        [SerializeField] private List<SceneEvents> _sceneEvents;

        public static bool IsLoaded;
        private bool _isMusicMuted;
        private JukeboxSceneType _activeSceneType;

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

        private void OnEnable() {
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        private void OnDisable() {
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
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
                SceneEvents events = this._sceneEvents.First(sceneEvent => sceneEvent.Type == this._activeSceneType);
                events.PlayEvent.Post(this.gameObject);
            }
        }
        
        /// <summary>
        /// Stops the main game music.
        /// </summary>
        public void StopMusic() {
            SceneEvents events = this._sceneEvents.First(sceneEvent => sceneEvent.Type == this._activeSceneType);
            events.StopEvent.Post(this.gameObject);
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
            SceneEvents events = this._sceneEvents.First(sceneEvent => sceneEvent.Type == this._activeSceneType);
            events.PauseEvent.Post(this.gameObject);
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
            SceneEvents events = this._sceneEvents.First(sceneEvent => sceneEvent.Type == this._activeSceneType);
            events.UnpauseEvent.Post(this.gameObject);
        }
        
        #endregion

        #region Private Methods

        private void OnActiveSceneChanged(Scene current, Scene next) {
            JukeboxSceneType newType;
            switch (next.name) {
                case "MainMenu":
                case "Lobby":
                    newType = JukeboxSceneType.Lobby;
                    break;
                case "Game":
                default:
                    newType = JukeboxSceneType.Game;
                    break;
            }

            if (newType != this._activeSceneType) {
                this.StopMusic();
                this._activeSceneType = newType;
                this.PlayMusic();
            }
            else {
                this._activeSceneType = newType;
            }
        }

        #endregion
    }
}
