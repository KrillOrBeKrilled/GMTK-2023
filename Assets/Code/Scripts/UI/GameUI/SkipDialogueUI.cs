using KrillOrBeKrilled.Core.Managers;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// SkipDialogueUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the skip dialogue UI.
    /// </summary>
    public class SkipDialogueUI : MonoBehaviour {
        [Tooltip("Used to skip the current dialogue event on user interaction.")]
        [SerializeField] private UIButton _uiButton;
        private UnityAction _onSkipComplete;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            if (PlayerPrefsManager.ShouldSkipDialogue()) {
                this.gameObject.SetActive(false);
            }
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Sets up references and listeners to notify observers of the skip dialogue completion and track
        /// when the level starts.
        /// </summary>
        /// <param name="onSkipComplete"> Tracks when the skip dialogue timer is completed. </param>
        public void Initialize(UnityAction onSkipComplete) {
            this._onSkipComplete = onSkipComplete;
        }

        /// <summary>
        /// Invokes the method associated with completing the dialogue skipping process set upon <see cref="Initialize"/>.
        /// </summary>
        public void TriggerSkip() {
            this._onSkipComplete?.Invoke();
        }

        /// <summary>
        /// Disables this GameObject.
        /// </summary>
        /// <remarks> Subscribed to the onStartLevel event provided upon <see cref="Initialize"/>. </remarks>
        public void OnStartLevel() {
            this._uiButton.gameObject.SetActive(false);
        }

        #endregion
    }
}
