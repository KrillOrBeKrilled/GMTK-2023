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
        internal PlayerInputActions PlayerInputActions { get; private set; }
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        protected override void Awake() {
            base.Awake();
            this.PlayerInputActions = new PlayerInputActions();
            this.PlayerInputActions.Enable();
        }
        
        #endregion

        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        // To help with UI stuff when disabling and enabling controls
        /// <summary>
        /// Disables the input retrieval through the <see cref="PlayerInputActions"/> asset.
        /// </summary>
        internal void DisablePlayerControls() {
            this.PlayerInputActions.Player.Disable();
        }
        
        /// <summary>
        /// Disables the pause input retrieval through the <see cref="PlayerInputActions"/> asset.
        /// </summary>
        internal void DisableUIControls() {
            this.PlayerInputActions.Pause.Disable();
        }

        /// <summary>
        /// Enables the input retrieval through the <see cref="PlayerInputActions"/> asset.
        /// </summary>
        internal void EnablePlayerControls() {
            this.PlayerInputActions.Player.Enable();
        }

        /// <summary>
        /// Enables the pause input retrieval through the <see cref="PlayerInputActions"/> asset.
        /// </summary>
        internal void EnableUIControls() {
            this.PlayerInputActions.Pause.Enable();
        }
        
        #endregion
    }
}
