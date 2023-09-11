using UnityEngine;
using Yarn.Unity;

public class CameraSwitcher : MonoBehaviour {
  public GameObject PlayerCamera;
  public GameObject StartCamera;
  public GameObject EndCamera;

  [YarnCommand("show_player")]
  public void ShowPlayer() {
    this.DisableAll();
    this.PlayerCamera.SetActive(true);
  }

  [YarnCommand("show_start")]
  public void ShowStart() {
    this.DisableAll();
    this.StartCamera.SetActive(true);
  }

  [YarnCommand("show_end")]
  public void ShowEnd() {
    this.DisableAll();
    this.EndCamera.SetActive(true);
  }

  private void DisableAll() {
    this.PlayerCamera.SetActive(false);
    this.StartCamera.SetActive(false);
    this.EndCamera.SetActive(false);
  }
}
