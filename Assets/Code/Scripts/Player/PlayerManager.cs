using UnityEngine;

namespace Player {
  public class PlayerManager : MonoBehaviour {
    public PlayerController PlayerController { get; private set; }
    public TrapController TrapController { get; private set; }

    private void Awake() {
      this.PlayerController = this.GetComponent<PlayerController>();
      this.TrapController = this.GetComponent<TrapController>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
      if (other.gameObject.layer != LayerMask.NameToLayer("Hero"))
        return;

      this.PlayerController.GameOver();
    }
  }
}
