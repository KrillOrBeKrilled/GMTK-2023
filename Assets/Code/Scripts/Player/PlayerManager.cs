using UnityEngine;

namespace Player {
  public class PlayerManager : MonoBehaviour {
    public PlayerController PlayerController { get; private set; }
    public TrapController TrapController { get; private set; }

    public float MapPosition => (this.transform.position.x - this._levelStart.position.x) / (this._levelEnd.position.x - this._levelStart.position.x);

    private Transform _levelStart;
    private Transform _levelEnd;

    public void Initialize(Transform levelStart, Transform levelEnd) {
      this._levelStart = levelStart;
      this._levelEnd = levelEnd;
    }

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
