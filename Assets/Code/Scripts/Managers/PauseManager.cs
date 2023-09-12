using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//*******************************************************************************************
// PauseManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Manages the pausing and unpausing mechanics of the game through direct
    /// manipulation of <see cref="Time.timeScale"/>.
    /// </summary>
    public class PauseManager: Singleton<PauseManager> {
        [Tooltip("Tracks when the game is paused and unpaused.")]
        public UnityEvent<bool> OnPauseToggled;

        // Denotes if the game can be paused.
        private bool _isPausable;
        // Denotes whether or not the game is currently paused.
        private bool _isPaused;

        private PlayerInputActions _playerInputActions;

        /// <summary>
        /// Enables or disables the ability to pause the game.
        /// </summary>
        /// <param name="isPausable"> Whether or not the game can be paused. </param>
        public void SetIsPausable(bool isPausable) {
            this._isPausable = isPausable;
        }

        /// <summary> Resets the <see cref="Time.timeScale"/> and resets <see cref="_isPaused"/>. </summary>
        /// <remarks> Invokes the <see cref="OnPauseToggled"/> event. </remarks>
        public void UnpauseGame() {
            Time.timeScale = 1f;
            this._isPaused = false;
            this.OnPauseToggled?.Invoke(this._isPaused);
        }

        protected override void Awake() {
            base.Awake();

            this._playerInputActions = new PlayerInputActions();
            this.OnPauseToggled = new UnityEvent<bool>();
        }

        private void OnEnable() {
            this._playerInputActions.Enable();
            this._playerInputActions.Pause.PauseAction.performed += this.TogglePausedState;
        }

        private void OnDisable() {
            this._playerInputActions.Disable();
            this._playerInputActions.Pause.PauseAction.performed += this.TogglePausedState;
        }

        /// <summary>
        /// Pauses the game when it's not already paused and vice versa, provided that <see cref="_isPausable"/>
        /// is toggled.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="PlayerInputActions"/> pause input event. </remarks>
        private void TogglePausedState(InputAction.CallbackContext ctx) {
            if (!this._isPausable) {
                return;
            }

            if (this._isPaused) {
                this.UnpauseGame();
            } else {
                this.PauseGame();
            }
        }

        /// <summary> Sets the <see cref="Time.timeScale"/> to zero and toggles <see cref="_isPaused"/>. </summary>
        /// <remarks> Invokes the <see cref="OnPauseToggled"/> event. </remarks>
        private void PauseGame() {
            Time.timeScale = 0f;
            this._isPaused = true;
            this.OnPauseToggled?.Invoke(this._isPaused);
        }
    }
}
