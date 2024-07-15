using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Core.Managers;
using UnityEngine;

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
        [SerializeField] private GameEvent _onSkipDialogue;

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
        /// Triggers referenced _onSkipDialogue <see cref="GameEvent"/>.
        /// </summary>
        public void TriggerSkip() {
            this._onSkipDialogue.Raise();
        }

        /// <summary>
        /// Disables this GameObject.
        /// </summary>
        /// <remarks> Triggered by onStartLevel <see cref="GameEvent"/>. </remarks>
        public void OnStartLevel() {
            this._uiButton.gameObject.SetActive(false);
        }

        #endregion
    }
}
