using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class CameraShaker : MonoBehaviour
{
    public float ShakeAmplitude = 0.5f;

    private CameraSwitcher _switcher;
    private List<CinemachineBasicMultiChannelPerlin> _noiseControllers;

    void Awake()
    {
        TryGetComponent(out _switcher);
        
        // record all camera noise controllers
        _noiseControllers = new();
        CinemachineVirtualCamera vcam;
        _switcher.HeroCamera.TryGetComponent(out vcam);
        _noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        _switcher.StartCamera.TryGetComponent(out vcam);
        _noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        _switcher.EndCamera.TryGetComponent(out vcam);
        _noiseControllers.Add(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
    }

    [YarnCommand("start_shake")]
    public void StartShake()
    {
        foreach (var noise in _noiseControllers)
        {
            noise.m_AmplitudeGain = ShakeAmplitude;
        }
    }
    
    [YarnCommand("stop_shake")]
    public void StopShake()
    {
        foreach (var noise in _noiseControllers)
        {
            noise.m_AmplitudeGain = 0;
        }
    }
}
