using UnityEngine;

//*******************************************************************************************
// FollowTarget
//*******************************************************************************************
namespace KrillOrBeKrilled.Common {
    /// <summary>
    /// Follows the position of a specified target with an offset with customizations for
    /// the axes that movement should be tracked along.
    /// </summary>
    public class FollowTarget : MonoBehaviour {
        [Tooltip("Transform of the target that the GameObject associated with this class should follow.")]
        [SerializeField] private Transform _followTarget;
    
        [Tooltip("Toggles whether the GameObject associated with this class should follow the position along the x-axis.")]
        [SerializeField] private bool _followTargetX = true;
        [Tooltip("Toggles whether the GameObject associated with this class should follow the position along the y-axis.")]
        [SerializeField] private bool _followTargetY = true;
        [Tooltip("Toggles whether the GameObject associated with this class should follow the position along the z-axis.")]
        [SerializeField] private bool _followTargetZ = true;
        [Tooltip("Offset from the position of the target.")]
        [SerializeField] private Vector3 _followOffset;
    
        private Transform _transform;
    
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this._transform = this.transform;
        }
    
        private void Update() {
            if (this._followTarget is null) {
                this.gameObject.SetActive(false);
                return;
            }
    
            var newPosition = this._transform.position;
    
            if (this._followTargetX) {
                newPosition.x = this._followTarget.position.x + this._followOffset.x;
            }
    
            if (this._followTargetY) {
                newPosition.y = this._followTarget.position.y + this._followOffset.y;
            }
    
            if (this._followTargetZ) {
                newPosition.z = this._followTarget.position.z + this._followOffset.z;
            }
    
            this._transform.position = newPosition;
        }
        
        #endregion
    }
}

