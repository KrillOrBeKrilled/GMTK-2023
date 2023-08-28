using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// CameraShaker
//*******************************************************************************************
/// <summary>
/// Handles adjustments to the Perlin Noise applied to all the cameras managed by the
/// <see cref="CameraSwitcher"/>.
/// </summary>
/// <remarks> Access to the cameras is provided through the <see cref="CameraSwitcher"/>. </remarks>
public class CameraShaker : MonoBehaviour
{
    public float ShakeAmplitude = 0.5f;

    private CameraSwitcher _switcher;
    private List<CinemachineBasicMultiChannelPerlin> _noiseControllers;

    void Awake()
    {
        this.TryGetComponent(out this._switcher);

        // record all camera noise controllers
        this._noiseControllers = new();
        CinemachineVirtualCamera vcam;
        this._switcher.PlayerCamera.TryGetComponent(out vcam);
        this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        this._switcher.StartCamera.TryGetComponent(out vcam);
        this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        this._switcher.EndCamera.TryGetComponent(out vcam);
        this._noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
    }

    /// <summary>
    /// Triggers a camera shake via the <see cref="CinemachineBasicMultiChannelPerlin"/> for each
    /// camera associated with the CameraSwitcher.
    /// </summary>
    /// <remarks> Can be accessed as a YarnCommand. </remarks>
    [YarnCommand("start_shake")]
    public void StartShake()
    {
        foreach (var noise in this._noiseControllers)
        {
            noise.m_AmplitudeGain = this.ShakeAmplitude;
        }
    }

    /// <summary>
    /// Stops the camera shake via the <see cref="CinemachineBasicMultiChannelPerlin"/> for each
    /// camera associated with the CameraSwitcher.
    /// </summary>
    /// <remarks> Can be accessed as a YarnCommand. </remarks>
    [YarnCommand("stop_shake")]
    public void StopShake()
    {
        foreach (var noise in this._noiseControllers)
        {
            noise.m_AmplitudeGain = 0;
        }
    }
}
