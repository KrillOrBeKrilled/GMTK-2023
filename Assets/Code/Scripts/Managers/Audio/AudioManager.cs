using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************
// AudioManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers.Audio {
    /// <summary>
    /// Acts as a sound bank for all the game's SFX. Works hand in hand with
    /// <see cref="Jukebox"/> to provide methods for listening in on events invoked
    /// during gameplay that handle all the Wwise sound events.
    /// </summary>
    public class AudioManager : Singleton<AudioManager> {
        // ------------ UI Sound Effects -------------
        [Tooltip("SFX associated with the UI.")]
        [SerializeField] private AK.Wwise.Event
            _playUIConfirmEvent,
            _playUISelectEvent,
            _playUITileSelectMoveEvent,
            _playUITileSelectConfirmEvent,
            _playUIPauseEvent,
            _playUIUnpauseEvent;

        // --------- Dialogue Sound Effects ----------
        [Tooltip("SFX associated with the dialogue.")]
        [SerializeField] private AK.Wwise.Event
            _henDialogueEvent,
            _heroDialogueEvent,
            _bossDialogueEvent;

        // ----------- Hen Sound Effects -------------
        [Tooltip("SFX associated with the player.")]
        [SerializeField] private AK.Wwise.Event
            _startBuildEvent,
            _stopBuildEvent,
            _buildCompleteEvent,
            _henDeathEvent,
            _henFlapEvent;

        // ----------- Hero Sound Effects ------------
        [Tooltip("SFX associated with the hero.")]
        [SerializeField] private AK.Wwise.Event
            _heroHurtEvent,
            _heroJumpEvent,
            _heroDeathEvent;

        /// Tracks when the SFX should be muted.
        internal bool AreSfxMuted { get; private set; }

        /// Tracks the trap building status to manipulate the trap building SFX for durations of time.
        private bool _isBuilding;

        /// Manages all the BGM.
        private Jukebox _jukebox;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        protected override void Awake() {
            base.Awake();
            this.AreSfxMuted = PlayerPrefsManager.AreSfxMuted();
        }

        private void Start() {
            this._jukebox = GameObject.Find("Jukebox")?.GetComponent<Jukebox>();

            if (SceneManager.GetActiveScene().name == "Game") {
                PauseManager.Instance.OnPauseToggled.AddListener(this.ToggleJukeboxPause);
            }
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        #region Dialogue Sound Events Methods
        
        /// <summary>
        /// Plays SFX associated with the boss dialogue (Dogan).
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayBossDialogue(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._bossDialogueEvent.Post(audioSource);
            }
        }
        
        /// <summary>
        /// Plays SFX associated with the player dialogue (Hendall).
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHenDialogue(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._henDialogueEvent.Post(audioSource);
            }
        }

        /// <summary>
        /// Plays SFX associated with the hero dialogue.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHeroDialogue(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._heroDialogueEvent.Post(audioSource);
            }
        }

        #endregion
        
        #region Hen Sound Events Methods
        
        /// <summary>
        /// Plays SFX associated with building a trap after deployment to set it up.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayBuild(GameObject audioSource) {
            if (this.AreSfxMuted) {
                return;
            }

            if (!this._isBuilding) {
                this._isBuilding = true;
                this.StartCoroutine(this.PlayBuildSoundForDuration(11f));
            }
        }
        
        /// <summary>
        /// Plays SFX associated with completing the building of traps, setting them up.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayBuildComplete(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._buildCompleteEvent.Post(audioSource);
            }
        }
        
        /// <summary>
        /// Plays character SFX associated with the player death.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHenDeath(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._henDeathEvent.Post(audioSource);
            }
        }
        
        /// <summary>
        /// Plays character SFX associated with the player jumping.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHenJump(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._henFlapEvent.Post(audioSource);
            }
        }

        /// <summary>
        /// Stops SFX associated with building a trap after deployment to set it up.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void StopBuild(GameObject audioSource) {
            if (this.AreSfxMuted) {
                return;
            }

            this.StopCoroutine(this.PlayBuildSoundForDuration(11f));
            this._stopBuildEvent.Post(this.gameObject);

            this._isBuilding = false;
        }

        #endregion
        
        #region Hero Sound Events Methods
        
        /// <summary>
        /// Plays character SFX associated with the hero dying.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHeroDeath(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._heroDeathEvent.Post(audioSource);
            }
        }
        
        /// <summary>
        /// Plays SFX associated with the hero taking damage.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHeroHurt(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._heroHurtEvent.Post(audioSource);
            }
        }

        /// <summary>
        /// Plays character SFX associated with the hero jumping.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayHeroJump(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._heroJumpEvent.Post(audioSource);
            }
        }

        #endregion
        
        #region UI Sound Events Methods
        
        /// <summary>
        /// Plays SFX associated with pressing a UI button.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        public void PlayUIClick(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._playUIConfirmEvent.Post(audioSource);
            }
        }

        /// <summary> 
        /// Deprecated: Plays SFX associated with hovering over or selecting a UI button. 
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        /// <remarks> Deprecated due to transitioning to Mobile target. </remarks>
        internal void PlayUIHover(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._playUISelectEvent.Post(audioSource);
            }
        }
        
        /// <summary>
        /// Plays SFX associated with deploying a trap on selected tile spaces.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayUITileSelectConfirm(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._playUITileSelectConfirmEvent.Post(audioSource);
            }
        }

        /// <summary>
        /// Plays SFX associated with changing the selected tile for deployment on the tilemap grid.
        /// </summary>
        /// <param name="audioSource"> The GameObject that's the source of this SFX. </param>
        internal void PlayUITileSelectMove(GameObject audioSource) {
            if (!this.AreSfxMuted) {
                this._playUITileSelectMoveEvent.Post(audioSource);
            }
        }

        #endregion
        
        internal void SetAreSfxMuted(bool areSfxMuted) {
            this.AreSfxMuted = areSfxMuted;
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Plays SFX associated with building a trap to set it up for a duration of time.
        /// </summary>
        /// <param name="durationInSeconds"> The duration of time to play the build SFX. </param>
        /// <remarks>
        /// The coroutine is started and stopped by <see cref="PlayBuild"/> and <see cref="StopBuild"/>.
        /// </remarks>
        private IEnumerator PlayBuildSoundForDuration(float durationInSeconds) {
            while (this._isBuilding) {
                this._startBuildEvent.Post(this.gameObject);
                yield return new WaitForSeconds(durationInSeconds);
                this._stopBuildEvent.Post(this.gameObject);
            }
        }
        
        /// <summary>
        /// Plays SFX for pausing the game and pauses the BGM through the <see cref="Jukebox"/>. If the
        /// game is already paused, plays SFX for unpausing the game and resumes the BGM instead.
        /// </summary>
        /// <param name="isPaused"> Denotes if the game is currently paused. </param>
        /// <remarks> Subscribed to the <see cref="PauseManager.OnPauseToggled"/> event. </remarks>
        private void ToggleJukeboxPause(bool isPaused) {
            if (this._jukebox is null) {
                return;
            }

            if (isPaused) {
                this._jukebox.PauseMusic();

                if (!this.AreSfxMuted) {
                    this._playUIPauseEvent.Post(this.gameObject);
                }
                return;
            }

            if (!this.AreSfxMuted) {
                this._playUIUnpauseEvent.Post(this.gameObject);
            }
            this._jukebox.UnpauseMusic();
        }
        
        #endregion
    }
}
