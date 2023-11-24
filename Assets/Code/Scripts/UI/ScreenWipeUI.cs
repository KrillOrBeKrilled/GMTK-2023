using System;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// ScreenWipeUI
//*******************************************************************************************
namespace KrillOrBeKrilled {
    [RequireComponent(typeof(Image))]
    public class ScreenWipeUI : MonoBehaviour {
        private Animator _animationController;
        private Image _image;

        private readonly int _shaderScaleKey = Shader.PropertyToID("_WipeScale");
        public float ScreenWipeSize = 0f;
        
        // Start is called before the first frame update
        void Start() {
            _image = GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update() {
            _image.materialForRendering.SetFloat(_shaderScaleKey, ScreenWipeSize);
        }
    }
}
