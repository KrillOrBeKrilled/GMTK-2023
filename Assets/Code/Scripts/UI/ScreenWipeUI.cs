using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// ScreenWipeUI
//*******************************************************************************************
namespace KrillOrBeKrilled {
    [RequireComponent(typeof(Image))]
    public class ScreenWipeUI : MonoBehaviour {
        private Image _image;
        
        private readonly int _shaderTextureKey = Shader.PropertyToID("_WipeShape");
        private readonly int _shaderScaleKey = Shader.PropertyToID("_WipeScale");

        [SerializeField] private List<Texture> _wipeShapes;
        public float ScreenWipeSize = 0f;

        private void Awake() {
            _image = GetComponent<Image>();
        }
        
        private void Update() {
            _image.material.SetFloat(_shaderScaleKey, ScreenWipeSize);
        }

        public void SetRandomWipeShape() {
            if (_wipeShapes.Count < 1) {
                return;
            }
            
            var randomWipeShape = Random.Range(0, _wipeShapes.Count);
            _image.materialForRendering.SetTexture(_shaderTextureKey, _wipeShapes[randomWipeShape]);
        }
    }
}
