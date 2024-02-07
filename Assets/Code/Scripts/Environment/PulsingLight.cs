using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//*******************************************************************************************
// PulsingLight
//*******************************************************************************************
namespace KrillOrBeKrilled.Environment  {
    /// <summary>
    /// Pulses a light in and out after a specified duration of time, lerping the light's
    /// outer radius and intensity to specified ranges.
    /// </summary>
    /// <remarks> To be used for point lights only. </remarks>
    public class PulsingLight : MonoBehaviour {

        public Light2D LightSource;

        public float MinIntensity, MaxIntensity;
        public float MinOuterRange, MaxOuterRange;
        
        public float TimeUntilPulse;
        private float _pulseTimer;

        private bool _isMaxIntensity;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            _pulseTimer = 0f;
        }
        
        private void Update() {
            _pulseTimer -= Time.deltaTime;

            if (!(_pulseTimer < 0f)) return;

            if (_isMaxIntensity) {
                DOVirtual
                    .Float(MinIntensity, MaxIntensity, TimeUntilPulse, newIntensity => 
                        LightSource.intensity = newIntensity)
                    .SetEase(Ease.Linear);
                
                DOVirtual
                    .Float(MinOuterRange, MaxOuterRange, TimeUntilPulse, newRange => 
                        LightSource.pointLightOuterRadius = newRange)
                    .SetEase(Ease.Linear);
            } else {
                DOVirtual
                    .Float(MaxIntensity, MinIntensity, TimeUntilPulse, newIntensity => 
                        LightSource.intensity = newIntensity)
                    .SetEase(Ease.Linear);
                
                DOVirtual
                    .Float(MaxOuterRange, MinOuterRange, TimeUntilPulse, newRange => 
                        LightSource.pointLightOuterRadius = newRange)
                    .SetEase(Ease.Linear);
            }
            
            _pulseTimer = TimeUntilPulse;
            _isMaxIntensity = !_isMaxIntensity;
        }
        
        #endregion
    }
}
