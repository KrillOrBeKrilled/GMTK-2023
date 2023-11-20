using KrillOrBeKrilled.Managers;
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
        [SerializeField] private UIHoldButton _uiHoldButton;
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
        /// <param name="onStartLevel"> Tracks when the level begins. </param>
        /// <param name="onSkipComplete"> Tracks when the skip dialogue timer is completed. </param>
        public void Initialize(UnityEvent onStartLevel, UnityAction onSkipComplete) {
            this._onSkipComplete = onSkipComplete;
            onStartLevel.AddListener(this.OnStartLevel);
        }

        public void TriggerSkip() {
            this._onSkipComplete?.Invoke();
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Disables this GameObject.
        /// </summary>
        /// <remarks> Subscribed to the onStartLevel event provided upon <see cref="Initialize"/>. </remarks>
        private void OnStartLevel() {
            this._uiHoldButton.HideOnCompletion();
        }

        #endregion
    }
}
