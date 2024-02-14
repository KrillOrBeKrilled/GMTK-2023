using System;
using System.IO;
using KrillOrBeKrilled.Core.Managers;
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
    /// <remarks>
    /// Manages the player GameObject as an entity that interacts with other GameObjects
    /// in the environment and acts as an intermediary to grant access to the
    /// <see cref="PlayerCharacter"/>; and <see cref="TrapController"/> to other classes.
    /// </remarks>
    public class PlayerController : MonoBehaviour {
        [Tooltip("The PlayerCharacter associated with the player GameObject.")]
        internal PlayerCharacter Player { get; private set; }
        
        [Tooltip("The TrapController associated with the player GameObject.")]
        internal TrapController TrapController { get; private set; }
        
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
        }
        
        // Take care of initializing the Player 
        
        // Start is called before the first frame update
        void Start() {
        
        }
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================

        #region Protected Methods

        public void Initialize(GameManager gameManager) {
            Player.Initialize(gameManager.OnHenWon, this.GatherInput);
        }

        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
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
        
        // To help with UI stuff when disabling and enabling controls
        /// <summary>
        /// Disables the input retrieval through the <see cref="PlayerInputActions"/> asset and unsubscribes all
        /// the input methods from the &lt;see cref="PlayerInputActions"/&gt; input action bindings.
        /// </summary>
        private void DisablePlayerControls() {
            this._playerInputActions.Player.Disable();
            
            this._playerInputActions.Player.PlaceTrap.performed -= this.DeployTrap;
        }

        /// <summary>
        /// Enables the input retrieval through the <see cref="PlayerInputActions"/> asset and subscribes all
        /// the input methods to the &lt;see cref="PlayerInputActions"/&gt; input action bindings.
        /// </summary>
        private void EnablePlayerControls() {
            this._playerInputActions.Player.Enable();
            
            this._playerInputActions.Player.PlaceTrap.performed += this.DeployTrap;
        }

        private void DeployTrap(InputAction.CallbackContext obj) {
            this.Player.InvokeDeployTrap();
        }

        /// <summary>
        /// Reads input for move and jump. Puts read values in the <c>out</c> variables.
        /// </summary>
        /// <param name="moveInput"> Move input is saved into this variable. </param>
        /// <param name="jumpPressed"> Jump Button pressed is saved into this variable. </param>
        /// <param name="jumpPressedThisFrame"> Jump Button pressed this frame is saved into this variable. </param>
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
