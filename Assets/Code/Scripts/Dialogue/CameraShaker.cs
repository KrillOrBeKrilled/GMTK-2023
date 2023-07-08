using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class CameraShaker : MonoBehaviour
{
    public float ShakeAmplitude = 0.5f;
    
    private CinemachineVirtualCamera _vcam;
    private CinemachineBasicMultiChannelPerlin _noise;

    void Awake()
    {
        TryGetComponent(out _vcam);
        _noise = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    [YarnCommand("start_shake")]
    public void StartShake()
    {
        _noise.m_AmplitudeGain = ShakeAmplitude;
    }
    
    [YarnCommand("stop_shake")]
    public void StopShake()
    {
        _noise.m_AmplitudeGain = 0;
    }
}
