using UnityEngine;
using Yarn.Unity;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject PlayerCamera;
    public GameObject StartCamera;
    public GameObject EndCamera;

    private void Start() => ShowStart();

    private void DisableAll()
    {
        this.PlayerCamera.SetActive(false);
        StartCamera.SetActive(false);
        EndCamera.SetActive(false);
    }

    [YarnCommand("show_player")]
    public void ShowPlayer()
    {
        DisableAll();
        this.PlayerCamera.SetActive(true);
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
