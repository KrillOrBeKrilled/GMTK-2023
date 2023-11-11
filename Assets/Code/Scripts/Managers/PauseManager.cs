using UnityEngine;

//*******************************************************************************************
// PauseManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Manages the pausing and un-pausing mechanics of the game through direct
    /// manipulation of <see cref="Time.timeScale"/>.
    /// </summary>
    public class PauseManager : MonoBehaviour {
        // Denotes if the game can be paused.
        private bool _isPausable;
        // Denotes whether or not the game is currently paused.
        private bool _isPaused;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary> Sets the <see cref="Time.timeScale"/> to zero and toggles <see cref="_isPaused"/>. </summary>
        /// <remarks> Invokes the <see cref="PauseToggledEvent"/> event. </remarks>
        public void PauseGame() {
            if (!this._isPausable) {
                return;
            }

            Time.timeScale = 0f;
            this._isPaused = true;
            EventManager.Instance.PauseToggledEvent?.Invoke(this._isPaused);
        }

        /// <summary>
        /// Enables or disables the ability to pause the game.
        /// </summary>
        /// <param name="isPausable"> Whether or not the game can be paused. </param>
        public void SetIsPausable(bool isPausable) {
            this._isPausable = isPausable;
        }

        /// <summary> Resets the <see cref="Time.timeScale"/> and resets <see cref="_isPaused"/>. </summary>
        /// <remarks> Invokes the <see cref="PauseToggledEvent"/> event. </remarks>
        public void UnpauseGame() {
            Time.timeScale = 1f;
            this._isPaused = false;
            EventManager.Instance.PauseToggledEvent?.Invoke(this._isPaused);
        }

        #endregion
    }
}
