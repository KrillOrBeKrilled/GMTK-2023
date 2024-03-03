using UnityEngine;

//*******************************************************************************************
// CameraCheckpoint
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Cameras {
    /// <summary>
    /// Relinquishes player camera control to the specified camera when the player comes
    /// in contact with the associated GameObject collider.
    /// </summary>
    public class CameraCheckpoint : MonoBehaviour {
        [Tooltip("The camera to activate when the player enters the collider area.")]
        public GameObject LevelCamera;
        
        [Tooltip("The camera manager's switcher in charge of switching virtual camera focal points.")]
        [SerializeField]
        private CameraSwitcher _cameraSwitcher;

        public bool FreezeOnEntry, FreezeOnExit;
        public float FreezeEntryTime, FreezeExitTime;
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void OnTriggerEnter2D(Collider2D other) {
            if (FreezeOnEntry) {
                this._cameraSwitcher.InvokeFreezeTransition(this.FreezeEntryTime);
            }
            
            this._cameraSwitcher.ShowLevelCamera(LevelCamera);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (FreezeOnExit) {
                this._cameraSwitcher.InvokeFreezeTransition(this.FreezeExitTime);
            }
            
            this._cameraSwitcher.ShowPlayer();
        }

        #endregion
    }
}
