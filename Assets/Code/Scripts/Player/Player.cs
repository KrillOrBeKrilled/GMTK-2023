using Input;
using UnityEngine;

public class Player : MonoBehaviour {
  public PlayerController PlayerController { get; private set; }

  public void MovePlayer(Vector2 newPosition) {
    this.transform.position = newPosition;
  }

  private void Awake() {
    this.PlayerController = this.GetComponent<PlayerController>();
  }

  private void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.layer != LayerMask.NameToLayer("Hero"))
      return;

    this.PlayerController.GameOver();
  }
}
