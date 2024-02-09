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

        [Range(0, 50)]
        public float MinIntensity, MaxIntensity;
        [Range(0, 50)]
        public float MinOuterRange, MaxOuterRange;
        
        [Range(0, 20)]
        public float TimeUntilPulse;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            Tween increaseIntensity = DOVirtual
                .Float(MinIntensity, MaxIntensity, TimeUntilPulse, newIntensity =>
                  LightSource.intensity = newIntensity)
                .SetEase(Ease.Linear);
            
              Tween increaseRange = DOVirtual
                .Float(MinOuterRange, MaxOuterRange, TimeUntilPulse, newRange =>
                  LightSource.pointLightOuterRadius = newRange)
                .SetEase(Ease.Linear);
            
              Tween decreaseIntensity = DOVirtual
                .Float(MaxIntensity, MinIntensity, TimeUntilPulse, newIntensity =>
                  LightSource.intensity = newIntensity)
                .SetEase(Ease.Linear);
            
              Tween decreaseRange = DOVirtual
                .Float(MaxOuterRange, MinOuterRange, TimeUntilPulse, newRange =>
                  LightSource.pointLightOuterRadius = newRange)
                .SetEase(Ease.Linear);
            
              Sequence sequence = DOTween.Sequence();
              sequence.Append(increaseIntensity);
              sequence.Join(increaseRange);
              sequence.Append(decreaseIntensity);
              sequence.Join(decreaseRange);
              sequence.SetLoops(-1);
              sequence.Play();
        }
        
        #endregion
    }
}
