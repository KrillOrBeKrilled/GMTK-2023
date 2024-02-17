using UnityEngine;

//*******************************************************************************************
// ParallaxEffect
//*******************************************************************************************
namespace KrillOrBeKrilled.Cameras {
    /// <summary>
    /// Manages the GameObject's transform x-axis offset with respect to changes to the
    /// main camera's transform to preserve a parallax effect throughout gameplay.
    /// </summary>
    public class ParallaxEffect : MonoBehaviour {
        [Tooltip("A percentage value clamped between [0,1] that is used to determine the extent of the " +
                 "gameObject's x-position offset from the camera position.")]
        [SerializeField] private float _parallaxMultiplier;
        [Tooltip("Used to correct the GameObject x-position offset corresponding to negative or positive camera movement.")]
        [SerializeField] private float _length;

        private Vector3 _lastCameraPosition;
        private Transform _transform;
        private Transform _cameraTransform;

        private float _startPos;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            this._transform = this.transform;
            this._cameraTransform = Camera.main!.transform;
            this._startPos = this._transform.position.x;
        }

        private void Update() {
            float cameraPosX = this._cameraTransform.position.x;
            float dist = cameraPosX * this._parallaxMultiplier;
            float temp = cameraPosX * (1 - this._parallaxMultiplier);

            Vector3 newPos = this._transform.position;
            newPos.x = this._startPos + dist;
            this._transform.position = newPos;

            if (temp > this._startPos + this._length) {
                this._startPos += this._length;
            } else if (temp < this._startPos - this._length) {
                this._startPos -= this._length;
            }
        }
        
        #endregion
    }
}
