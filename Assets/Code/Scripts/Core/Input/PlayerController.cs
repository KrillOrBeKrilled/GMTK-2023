using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using KrillOrBeKrilled.Core.Managers;
using KrillOrBeKrilled.Player;

//*******************************************************************************************
// PlayerController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Input {
    /// <summary>
    /// Manages the enabling, disabling, and delegation of character controls to the
    /// possessed <see cref="PlayerCharacter"/> directly through
    /// <see cref="PlayerInputActions"/>.
    /// </summary>
    /// <remarks>
    /// Manages the player GameObject as an entity that interacts with other GameObjects
    /// in the environment and acts as an intermediary to grant access to the
    /// <see cref="PlayerCharacter"/>; and <see cref="TrapController"/> to core gameplay
    /// management classes.
    /// </remarks>
    public class PlayerController : MonoBehaviour {
        [Tooltip("The PlayerCharacter associated with the player GameObject.")]
        internal PlayerCharacter Player { get; private set; }
        
        [Tooltip("The TrapController associated with the player GameObject.")]
        internal TrapController TrapController { get; private set; }
        
        // ----------------- Input -------------------
        private PlayerInputActions _playerInputActions;
        
        // ---------------- Replaying ----------------
        protected InputEventTrace InputRecorder;
        private const string BaseFolder = "InputRecordings";
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        protected virtual void Awake() {
            this.Player = this.GetComponent<PlayerCharacter>();
            this.TrapController = this.GetComponent<TrapController>();
            
            this._playerInputActions = new PlayerInputActions();
            this._playerInputActions.Enable();
            
            this.InputRecorder = new InputEventTrace();
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Sets up the <see cref="PlayerCharacter"/> and <see cref="TrapController"/> that make up the player
        /// representation within the game world with callbacks and delegates required for proper execution.
        /// </summary>
        /// <param name="gameManager"> Provides major game state-related events to subscribe the entities to. </param>
        internal void Initialize(GameManager gameManager) {
            Player.Initialize(gameManager.OnHenWon, this.GatherInput);
            TrapController.Initialize(ResourceManager.Instance.CanAffordCost);
        }
        
        /// <summary>
        /// Begins recording all player input with timestamps via the <see cref="InputEventTrace"/> and enables
        /// all controls from the <see cref="PlayerInputActions"/>.
        /// </summary>
        internal virtual void StartSession() {
            this.InputRecorder.Enable();
            this.EnablePlayerControls();
        }

        /// <summary>
        /// Stops recording play input via the <see cref="InputEventTrace"/>, creates a file for the recorded input,
        /// and frees the memory used for the recording process.
        /// </summary>
        internal virtual void StopSession() {
            this.InputRecorder.Disable();
            this.DisablePlayerControls();
            
            this.CreateRecordingFile();

            // Prevent memory leaks!
            this.InputRecorder.Dispose();
        }

        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods
        
        /// <summary>
        /// Disables the input retrieval through the <see cref="PlayerInputActions"/> asset and unsubscribes all
        /// the input methods from the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void DisablePlayerControls() {
            this._playerInputActions.Player.Disable();
            
            this._playerInputActions.Player.PlaceTrap.performed -= this.DeployTrap;
        }

        /// <summary>
        /// Enables the input retrieval through the <see cref="PlayerInputActions"/> asset and subscribes all
        /// the input methods to the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void EnablePlayerControls() {
            this._playerInputActions.Player.Enable();
            
            this._playerInputActions.Player.PlaceTrap.performed += this.DeployTrap;
        }

        /// <summary>
        /// Delegates trap deployment behaviour to the possessed <see cref="PlayerCharacter"/>.
        /// </summary>
        private void DeployTrap(InputAction.CallbackContext obj) {
            this.Player.InvokeDeployTrap();
        }

        /// <summary>
        /// Reads all input received via the <see cref="PlayerInputActions"/> and relays the values through the
        /// <c>out</c> variables.
        /// </summary>
        /// <param name="moveInput"> Move input (1D x-axis) is saved into this variable. </param>
        /// <param name="jumpPressed"> Jump Button pressed data is saved into this variable. </param>
        /// <param name="jumpPressedThisFrame"> Jump Button pressed this frame data is saved into this variable. </param>
        private void GatherInput(out float moveInput, out bool jumpPressed, out bool jumpPressedThisFrame) {
            Vector2 moveVectorInput = this._playerInputActions.Player.Move.ReadValue<Vector2>();
            moveInput = moveVectorInput.x;

            jumpPressed = this._playerInputActions.Player.Jump.IsPressed();
            jumpPressedThisFrame = this._playerInputActions.Player.Jump.WasPerformedThisFrame();
        }
        
        /// <summary>
        /// Helper method for creating a file for the player input recording based on platform the game was played on.
        /// </summary>
        /// <remarks>
        /// The recording files will be stored in <b>Assets > InputRecordings</b> within the project
        /// hierarchy. For a build, the recordings can be found in <b>Henchman_Data > InputRecordings</b>.
        /// </remarks>
        private void CreateRecordingFile() {
            var fileName = $"Playtest {DateTime.Now:MM-dd-yyyy HH-mm-ss}.txt";

            // From https://stackoverflow.com/questions/70715187/unity-read-and-write-to-txt-file-after-build
#if UNITY_EDITOR
            var path = Application.dataPath + $"/{BaseFolder}/" ;
            var path1 = Application.dataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#elif UNITY_ANDROID
            var path = Application.persistentDataPath + $"/{BaseFolder}/";
            var path1 = Application.persistentDataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#elif UNITY_IPHONE
            var path = Application.persistentDataPath + $"/{BaseFolder}/";
            var path1 = Application.persistentDataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#else
            var path = Application.dataPath + $"/{BaseFolder}/";
            var path1 = Application.dataPath + $"/{BaseFolder}";
            if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#endif

            this.InputRecorder.WriteTo(path + fileName);
        }

        #endregion
    }
}
