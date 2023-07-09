using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject HeroCamera;
    public GameObject StartCamera;
    public GameObject EndCamera;

    private void Start() => ShowStart();

    private void DisableAll()
    {
        HeroCamera.SetActive(false);
        StartCamera.SetActive(false);
        EndCamera.SetActive(false);
    }

    [YarnCommand("show_hero")]
    public void ShowHero()
    {
        DisableAll();
        HeroCamera.SetActive(true);
    }
    
    [YarnCommand("show_start")]
    public void ShowStart()
    {
        DisableAll();
        StartCamera.SetActive(true);
    }
    
    [YarnCommand("show_end")]
    public void ShowEnd()
    {
        DisableAll();
        EndCamera.SetActive(true);
    }
}
