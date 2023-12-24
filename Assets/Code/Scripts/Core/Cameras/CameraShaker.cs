using Cinemachine;
using System.Collections.Generic;
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
        [Tooltip("The Perlin Noise intensity to apply to the camera to cause camera shakes.")]
        public float ShakeAmplitude = 0.5f;
    
        /// Acts as the access point for the active cameras in the level.
        private CameraSwitcher _switcher;
        /// Noise control settings for each camera in the camera switcher.
        private List<CinemachineBasicMultiChannelPerlin> _noiseControllers;
    
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
        [YarnCommand("start_shake")]
        public void StartShake() {
            foreach (var noise in this._noiseControllers) {
                noise.m_AmplitudeGain = this.ShakeAmplitude;
            }
        }
    
        /// <summary>
        /// Stops the camera shake via the <see cref="CinemachineBasicMultiChannelPerlin"/> for each
        /// camera associated with the CameraSwitcher.
        /// </summary>
        /// <remarks> Can be accessed as the "stop_shake" YarnCommand. </remarks>
        [YarnCommand("stop_shake")]
        public void StopShake() {
            foreach (var noise in this._noiseControllers) {
                noise.m_AmplitudeGain = 0;
            }
        }
        
        #endregion
    }
}
