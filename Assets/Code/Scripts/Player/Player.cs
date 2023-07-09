using Input;
using UnityEngine;

public class Player : MonoBehaviour {
  private PlayerController _playerController;

  private void Awake() {
    this.TryGetComponent(out this._playerController);
  }

  private void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.layer != LayerMask.NameToLayer("Hero"))
      return;

    this._playerController.GameOver();
    GameManager.Instance.OnGameLost?.Invoke();
    Destroy(this.gameObject);
  }
}
