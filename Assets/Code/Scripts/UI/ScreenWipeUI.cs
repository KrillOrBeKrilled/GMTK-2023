using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// ScreenWipeUI
//*******************************************************************************************
namespace KrillOrBeKrilled {
    [RequireComponent(typeof(Image))]
    public class ScreenWipeUI : MonoBehaviour {
        [Tooltip("Exposes the screen wipe material WipeSize property to the Animator for direct manipulation.")]
        public float ScreenWipeSize = 0f;
        
        [SerializeField] private List<Texture> _wipeShapes;
        
        private Image _image;
        private readonly int _shaderTextureKey = Shader.PropertyToID("_WipeShape");
        private readonly int _shaderScaleKey = Shader.PropertyToID("_WipeScale");
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Awake() {
            _image = GetComponent<Image>();
        }
        
        private void Update() {
            _image.material.SetFloat(_shaderScaleKey, ScreenWipeSize);
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================

        #region Public Methods
        
        /// <summary>
        /// Sets a random screen wipe shape texture for the associated screen wipe material.
        /// </summary>
        public void SetRandomWipeShape() {
            if (_wipeShapes.Count < 1) {
                return;
            }
            
            var randomWipeShape = Random.Range(0, _wipeShapes.Count);
            _image.materialForRendering.SetTexture(_shaderTextureKey, _wipeShapes[randomWipeShape]);
        }
        
        #endregion
    }
}
