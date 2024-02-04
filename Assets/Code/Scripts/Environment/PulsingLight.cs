using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KrillOrBeKrilled.Environment  {
    public class PulsingLight : MonoBehaviour {

        public Light2D LightSource;

        public float MinIntensity, MaxIntensity;
        public float MinOuterRange, MaxOuterRange;
        
        public float TimeUntilPulse;
        private float _pulseTimer;

        private bool _isMaxIntensity;

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
    }
}
