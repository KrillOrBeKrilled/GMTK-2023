using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Managers.Audio;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// SettingsUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the updates from player interaction with the settings menu UI to the
    /// PlayerPref settings through the <see cref="PlayerPrefsManager"/>.
    /// </summary>
    public class SettingsUI : MonoBehaviour {
        [Tooltip("Used to enable or disable the level dialogue event playback settings.")]
        [SerializeField] private Toggle _skipDialogueToggle;
        [Tooltip("Used to enable or disable the game music settings.")]
        [SerializeField] private Toggle _muteMusicToggle;
        [Tooltip("Used to enable or disable the game SFX settings.")]
        [SerializeField] private Toggle _muteSfxToggle;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this._skipDialogueToggle.isOn = PlayerPrefsManager.ShouldSkipDialogue();
            this._skipDialogueToggle.onValueChanged.AddListener(this.OnSkipToggleValueChanged);

            this._muteMusicToggle.isOn = PlayerPrefsManager.IsMusicMuted();
            this._muteMusicToggle.onValueChanged.AddListener(this.OnMuteMusicToggleValueChanged);

            this._muteSfxToggle.isOn = PlayerPrefsManager.AreSfxMuted();
            this._muteSfxToggle.onValueChanged.AddListener(this.OnMuteSfxToggleValueChanged);
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Updates the mute music settings through <see cref="PlayerPrefsManager"/> and updates the music
        /// output accordingly.
        /// </summary>
        /// <param name="muteMusic"> Whether the option to mute the music is toggled or not. </param>
        /// <remarks> Subscribed to the <see cref="Toggle.onValueChanged"/> event for the
        /// <see cref="_muteMusicToggle"/>. </remarks>
        private void OnMuteMusicToggleValueChanged(bool muteMusic) {
            PlayerPrefsManager.SetMuteMusic(muteMusic);

            if (muteMusic) {
                Jukebox.Instance.StopMusic();
            } else {
                Jukebox.Instance.PlayMusic();
            }
        }

        /// <summary>
        /// Updates the mute SFX settings through <see cref="PlayerPrefsManager"/>.
        /// </summary>
        /// <param name="muteSfx"> Whether the option to mute the SFX is toggled or not. </param>
        /// <remarks> Subscribed to the <see cref="Toggle.onValueChanged"/> event for the
        /// <see cref="_muteSfxToggle"/>. </remarks>
        private void OnMuteSfxToggleValueChanged(bool muteSfx) {
            PlayerPrefsManager.SetMuteSfx(muteSfx);
        }
        
        /// <summary>
        /// Updates the skip dialogue sequence settings through <see cref="PlayerPrefsManager"/>.
        /// </summary>
        /// <param name="skipDialogue"> Whether the option to skip the dialogue sequence is toggled or not. </param>
        /// <remarks> Subscribed to the <see cref="Toggle.onValueChanged"/> event for the
        /// <see cref="_skipDialogueToggle"/>. </remarks>
        private void OnSkipToggleValueChanged(bool skipDialogue) {
            PlayerPrefsManager.SetSkipDialogue(skipDialogue);
        }
        
        #endregion
    }
}
