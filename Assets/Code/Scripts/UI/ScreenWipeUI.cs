using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// ScreenWipeUI
//*******************************************************************************************
namespace KrillOrBeKrilled {
    [RequireComponent(typeof(Image))]
    public class ScreenWipeUI : MonoBehaviour {
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
            _image.material.SetFloat(_shaderScaleKey, 500f);
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        public void WipeIn(UnityAction onComplete) {
            DOVirtual
                .Float(0, 500, 0.90f, newSize => 
                    _image.material.SetFloat(_shaderScaleKey, newSize))
                .SetEase(Ease.InExpo)
                .OnComplete(onComplete.Invoke);
        }
        
        public void WipeOut() {
            DOVirtual
                .Float(500, 0, 0.90f, newSize => 
                    _image.material.SetFloat(_shaderScaleKey, newSize))
                .SetEase(Ease.OutExpo)
                .OnComplete(() => this.gameObject.SetActive(false));
        }
        
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
