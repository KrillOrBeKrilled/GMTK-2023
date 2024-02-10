using UnityEngine;

namespace KrillOrBeKrilled.Cameras {
    public class ParallaxEffect : MonoBehaviour {
        [SerializeField] private float _parallaxMultiplier;
        [SerializeField] private float _length;

        private Vector3 _lastCameraPosition;
        private Transform _transform;
        private Transform _cameraTransform;

        private float _startPos;

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
    }
}
