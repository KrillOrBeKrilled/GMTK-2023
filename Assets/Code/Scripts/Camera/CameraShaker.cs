using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

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

    [YarnCommand("start_shake")]
    public void StartShake()
    {
        foreach (var noise in this._noiseControllers)
        {
            noise.m_AmplitudeGain = this.ShakeAmplitude;
        }
    }

    [YarnCommand("stop_shake")]
    public void StopShake()
    {
        foreach (var noise in this._noiseControllers)
        {
            noise.m_AmplitudeGain = 0;
        }
    }
}
