using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// ScreenWipeUI
//*******************************************************************************************
namespace KrillOrBeKrilled {
    /// <summary>
    /// Manages updates to the screen wipe transition effect, randomizing the display
    /// shapes and adjusting the shape scales.
    /// </summary>
    /// <remarks> Requires <see cref="Image"/> component. </remarks>
    
    [RequireComponent(typeof(Image))]
    public class ScreenWipeUI : MonoBehaviour {
        [Tooltip("Exposes the screen wipe material WipeSize property to the Animator for direct manipulation.")]
        public float ScreenWipeSize = 0f;
        
        [Tooltip("The outlines to be selected to display throughout the transition effects.")]
        [SerializeField] private List<Texture> _wipeShapes;
        
        private Image _image;
        private readonly int _shaderTextureKey = Shader.PropertyToID("_WipeShape");
        private readonly int _shaderScaleKey = Shader.PropertyToID("_WipeScale");
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Awake() {
            this._image = GetComponent<Image>();
        }
        
        private void Update() {
            this._image.material.SetFloat(_shaderScaleKey, ScreenWipeSize);
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
            if (this._wipeShapes.Count < 1) {
                return;
            }
            
            var randomWipeShape = Random.Range(0, this._wipeShapes.Count);
            this._image.materialForRendering.SetTexture(this._shaderTextureKey, this._wipeShapes[randomWipeShape]);
        }
        
        #endregion
    }
}
