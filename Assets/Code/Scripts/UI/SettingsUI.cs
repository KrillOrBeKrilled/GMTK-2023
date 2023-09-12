using KrillOrBeKrilled.Managers;
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
        [SerializeField] private Toggle _skipDialogueToggle;
        [SerializeField] private Toggle _muteMusicToggle;
        [SerializeField] private Toggle _muteSfxToggle;

        private void Awake() {
            this._skipDialogueToggle.isOn = PlayerPrefsManager.ShouldSkipDialogue();
            this._skipDialogueToggle.onValueChanged.AddListener(this.OnSkipToggleValueChanged);

            this._muteMusicToggle.isOn = PlayerPrefsManager.IsMusicMuted();
            this._muteMusicToggle.onValueChanged.AddListener(this.OnMuteMusicToggleValueChanged);

            this._muteSfxToggle.isOn = PlayerPrefsManager.AreSfxMuted();
            this._muteSfxToggle.onValueChanged.AddListener(this.OnMuteSfxToggleValueChanged);
        }

        /// <summary> Updates the skip dialogue sequence settings through <see cref="PlayerPrefsManager"/>. </summary>
        /// <param name="value"> Whether the option to skip the dialogue sequence is toggled or not. </param>
        /// <remarks> Subscribed to the <see cref="Toggle.onValueChanged"/> event for the
        /// <see cref="_skipDialogueToggle"/>. </remarks>
        private void OnSkipToggleValueChanged(bool skipDialogue) {
            PlayerPrefsManager.SetSkipDialogue(skipDialogue);
        }

        private void OnMuteMusicToggleValueChanged(bool muteMusic) {
            PlayerPrefsManager.SetMuteMusic(muteMusic);

            if (muteMusic) {
                Jukebox.Instance.StopMusic();
            } else {
                Jukebox.Instance.PlayMusic();
            }
        }

        private void OnMuteSfxToggleValueChanged(bool muteSfx) {
            PlayerPrefsManager.SetMuteSfx(muteSfx);
        }
    }
}
