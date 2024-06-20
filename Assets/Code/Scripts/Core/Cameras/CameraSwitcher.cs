using System.Collections.Generic;
using KrillOrBeKrilled.Common;
using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// CameraSwitcher
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Cameras {
    /// <summary>
    /// Handles the transitions between the various cameras used to focus on parts of the
    /// levels and its actors.
    /// </summary>
    /// <remarks> Manages and exposes all references to the cameras in a level. </remarks>
    public class CameraSwitcher : MonoBehaviour {
        [Tooltip("The camera focused on the player.")]
        public GameObject PlayerCamera;
        
        [Tooltip("The camera focused on the starting position of the level.")]
        public GameObject StartCamera;
    
        [Tooltip("The camera focused on the end position of the level.")]
        public GameObject EndCamera;
        
        [Tooltip("The cameras focused on specific points of interest in the level.")]
        [SerializeField]
        private List<GameObject> _levelCameras;

        [Tooltip("Tracks when a camera transition that freezes actor movement begins.")] 
        [SerializeField] private FloatEvent _onSwitchCameraFreeze;
        

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Enables only the <see cref="EndCamera"/> to transition the screen to focus on the level goal.
        /// </summary>
        /// <remarks> Can be accessed as the "show_end" YarnCommand. </remarks>
        [YarnCommand("show_end")]
        public void ShowEnd() {
            DisableAll();
            this.EndCamera.SetActive(true);
        }
        
        /// <summary>
        /// Enables only the <see cref="PlayerCamera"/> to transition the screen to focus on the player.
        /// </summary>
        /// <remarks> Can be accessed as the "show_player" YarnCommand. </remarks>
        [YarnCommand("show_player")]
        public void ShowPlayer() {
            DisableAll();
            this.PlayerCamera.SetActive(true);
        }
    
        /// <summary>
        /// Enables only the <see cref="StartCamera"/> to transition the screen to focus on the beginning of the level.
        /// </summary>
        /// <remarks> Can be accessed as the "show_start" YarnCommand. </remarks>
        [YarnCommand("show_start")]
        public void ShowStart() {
            DisableAll();
            this.StartCamera.SetActive(true);
        }
        
        /// <summary>
        /// Enables a level camera to transition the screen to a focal point of interest.
        /// </summary>
        /// <param name="levelCam"> The camera to set active. </param>
        public void ShowLevelCamera(GameObject levelCam) {
            DisableAll();
            levelCam.SetActive(true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="freezeTime"> The camera to set active. </param>
        public void InvokeFreezeTransition(float freezeTime) {
            this._onSwitchCameraFreeze.Raise(freezeTime);
        }

        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Disables every camera managed by this class.
        /// </summary>
        private void DisableAll() {
            this.PlayerCamera.SetActive(false);
            this.StartCamera.SetActive(false);
            this.EndCamera.SetActive(false);

            foreach (var cam in this._levelCameras) {
                cam.SetActive(false);
            }
        }
        
        #endregion
    }
}
