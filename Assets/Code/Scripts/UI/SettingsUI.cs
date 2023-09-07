using Managers;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// SettingsUI
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Handles the updates from player interaction with the settings menu UI to the
    /// PlayerPref settings through the <see cref="PlayerPrefsManager"/>.
    /// </summary>
    public class SettingsUI : MonoBehaviour {
        [SerializeField] private Toggle _skipDialogueToggle;

        private void Awake() {
            this._skipDialogueToggle.isOn = PlayerPrefsManager.ShouldSkipDialogue();
            this._skipDialogueToggle.onValueChanged.AddListener(this.OnSkipToggleValueChanged);
        }

        /// <summary> Updates the skip dialogue sequence settings through <see cref="PlayerPrefsManager"/>. </summary>
        /// <param name="value"> Whether the option to skip the dialogue sequence is toggled or not. </param>
        /// <remarks> Subscribed to the <see cref="Toggle.onValueChanged"/> event for the
        /// <see cref="_skipDialogueToggle"/>. </remarks>
        private void OnSkipToggleValueChanged(bool value) {
            PlayerPrefsManager.SetSkipDialogue(value);
        }
    }
}
