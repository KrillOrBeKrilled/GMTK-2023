using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using KrillOrBeKrilled.Player;

//*******************************************************************************************
// PlayerController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Input {
    /// <summary>
    /// Manages the enabling and disabling of character controls directly through
    /// <see cref="PlayerInputActions"/>.
    /// </summary>
    public class PlayerController : MonoBehaviour {

        [SerializeField]
        private PlayerCharacter player;
        
        [Tooltip("The PlayerInputActions asset associated with the player controller.")]
        internal PlayerInputActions PlayerInputActions { get; private set; }
        
        // ---------------- Replaying ----------------
        protected InputEventTrace InputRecorder;
        
        private const string BaseFolder = "InputRecordings";
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        protected virtual void Awake() {
            this.PlayerInputActions = new PlayerInputActions();
            this.PlayerInputActions.Enable();
        }
        
        // Take care of initializing the Player 
        
        // Start is called before the first frame update
        void Start() {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================

        #region Protected Methods
        
        /// <summary>
        /// Begins recording all player input with timestamps via the <see cref="InputEventTrace"/> and enables
        /// all controls from the <see cref="PlayerInputActions"/>.
        /// </summary>
        internal virtual void StartSession() {
            this.InputRecorder.Enable();

            this.EnableControls();
        }

        /// <summary>
        /// Stops recording play input via the <see cref="InputEventTrace"/>, creates a file for the recorded input,
        /// and frees the memory used for the recording process.
        /// </summary>
        /// <remarks>
        /// Subscribed to the <see cref="GameManager.OnHenWon"/> and <see cref="GameManager.OnHenLost"/> events.
        /// </remarks>
        internal virtual void StopSession() {
            this.InputRecorder.Disable();
            this.CreateRecordingFile();

            // Prevent memory leaks!
            this.InputRecorder.Dispose();
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
        /// Enables the input retrieval through the <see cref="PlayerInputActions"/> asset.
        /// </summary>
        internal void EnablePlayerControls() {
            this.PlayerInputActions.Player.Enable();
        }
        
        /// <summary>
        /// Unsubscribes all the input methods from the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void DisableControls() {
            this.PlayerInputActions.Player.PlaceTrap.performed -= this.DeployTrap;
        }

        /// <summary>
        /// Subscribes all the input methods to the <see cref="PlayerInputActions"/> input action bindings.
        /// </summary>
        private void EnableControls() {
            this.PlayerInputActions.Player.PlaceTrap.performed += this.DeployTrap;
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Executes the <see cref="DeployCommand"/>.
        /// </summary>
        private void DeployTrap(InputAction.CallbackContext obj) {
            this.player.InvokeDeployTrap();
        }

        /// <summary>
        /// Reads input for move and jump. Puts read values in the <c>out</c> variables.
        /// </summary>
        /// <param name="moveInput"> Move input is saved into this variable. </param>
        /// <param name="jumpPressed"> Jump Button pressed is saved into this variable. </param>
        /// <param name="jumpPressedThisFrame"> Jump Button pressed this frame is saved into this variable. </param>
        private void GatherInput(out float moveInput, out bool jumpPressed, out bool jumpPressedThisFrame) {
            Vector2 moveVectorInput = this.PlayerInputActions.Player.Move.ReadValue<Vector2>();
            moveInput = moveVectorInput.x;

            jumpPressed = this.PlayerInputActions.Player.Jump.IsPressed();
            jumpPressedThisFrame = this.PlayerInputActions.Player.Jump.WasPerformedThisFrame();
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
