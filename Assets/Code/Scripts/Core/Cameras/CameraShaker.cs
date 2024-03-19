using Cinemachine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// CameraShaker
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Cameras {
    /// <summary>
    /// Handles adjustments to the Perlin Noise applied to all the cameras managed by the
    /// <see cref="CameraSwitcher"/>.
    /// </summary>
    /// <remarks> Access to the cameras is provided through the
    /// <see cref="CameraSwitcher"/>.
    /// </remarks>
    public class CameraShaker : MonoBehaviour {
        /// Acts as the access point for the active cameras in the level.
        private CameraSwitcher _switcher;
        /// Noise control settings for each camera in the camera switcher.
        private List<CinemachineBasicMultiChannelPerlin> _noiseControllers;
        private Tween _camShakeTween;
    
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this.TryGetComponent(out this._switcher);
    
            // record all camera noise controllers
            this._noiseControllers = new List<CinemachineBasicMultiChannelPerlin>();
            
            CinemachineVirtualCamera vcam;
            this._switcher.PlayerCamera.TryGetComponent(out vcam);
            this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
            this._switcher.StartCamera.TryGetComponent(out vcam);
            this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
            this._switcher.EndCamera.TryGetComponent(out vcam);
            this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        }
        
        #endregion

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Triggers a camera shake via the <see cref="CinemachineBasicMultiChannelPerlin"/> for each
        /// camera associated with the CameraSwitcher.
        /// </summary>
        /// <remarks> Can be accessed as the "start_shake" YarnCommand. </remarks>
        /// <param name="shakeAmplitude"> The Perlin Noise intensity to apply to the camera to cause camera shakes. </param>
        /// <param name="duration"> The span of time that the camera shakes will last. </param>
        [YarnCommand("start_shake")]
        public void StartShake(float shakeAmplitude, float duration) {
            _camShakeTween = DOVirtual.Float(shakeAmplitude, 0, duration, amplitude => {
                foreach (var noise in this._noiseControllers) {
                    noise.m_AmplitudeGain = amplitude;
                }
            }).SetEase(Ease.Linear);
        }
    
        /// <summary>
        /// Stops the camera shake via the <see cref="CinemachineBasicMultiChannelPerlin"/> for each
        /// camera associated with the CameraSwitcher.
        /// </summary>
        /// <remarks> Can be accessed as the "stop_shake" YarnCommand. </remarks>
        [YarnCommand("stop_shake")]
        public void StopShake() {
            this._camShakeTween?.Kill();

            foreach (var noise in this._noiseControllers) {
                noise.m_AmplitudeGain = 0;
            }
        }
        
        #endregion
    }
}
