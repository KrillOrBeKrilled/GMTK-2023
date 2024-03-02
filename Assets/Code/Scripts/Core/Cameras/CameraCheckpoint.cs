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
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            this._cameraSwitcher.ShowLevelCamera(LevelCamera);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            this._cameraSwitcher.ShowPlayer();
        }

        #endregion
    }
}
