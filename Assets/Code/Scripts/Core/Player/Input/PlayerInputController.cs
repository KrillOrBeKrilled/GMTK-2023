using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Input;
using UnityEngine;

//*******************************************************************************************
// PlayerInputController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Manages the enabling and disabling of character controls directly through
    /// <see cref="PlayerInputActions"/>.
    /// </summary>
    public class PlayerInputController : Singleton<PlayerInputController> {
        [Tooltip("The PlayerInputActions asset associated with this player controller.")]
        public PlayerInputActions PlayerInputActions { get; private set; }

        protected override void Awake() {
            base.Awake();
            this.PlayerInputActions = new PlayerInputActions();
            this.PlayerInputActions.Enable();
        }

        // To help with UI stuff when disabling and enabling controls
        /// <summary> Disables the input retrieval through the <see cref="PlayerInputActions"/> asset. </summary>
        public void DisablePlayerControls() {
            this.PlayerInputActions.Player.Disable();
        }

        /// <summary> Enables the input retrieval through the <see cref="PlayerInputActions"/> asset. </summary>
        public void EnablePlayerControls() {
            this.PlayerInputActions.Player.Enable();
        }

        /// <summary> Disables the pause input retrieval through the <see cref="PlayerInputActions"/> asset. </summary>
        public void DisableUIControls() {
            this.PlayerInputActions.Pause.Disable();
        }

        /// <summary> Enables the pause input retrieval through the <see cref="PlayerInputActions"/> asset. </summary>
        public void EnableUIControls() {
            this.PlayerInputActions.Pause.Enable();
        }
    }
}
