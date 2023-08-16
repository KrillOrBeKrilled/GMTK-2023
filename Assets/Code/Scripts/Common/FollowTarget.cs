using UnityEngine;

public class FollowTarget : MonoBehaviour {
  [SerializeField] private Transform _followTarget;
  [SerializeField] private bool _followTargetX = true;
  [SerializeField] private bool _followTargetY = true;
  [SerializeField] private Vector2 _followOffset;

  private Transform _transform;

  private void Awake() {
    this._transform = this.transform;
  }

  private void Update() {
    if (this._followTarget == null) {
      this.gameObject.SetActive(false);
      return;
    }

    Vector2 newPosition = this._transform.position;

    if (this._followTargetX) {
      newPosition.x = this._followTarget.position.x + this._followOffset.x;
    }

    if (this._followTargetY) {
      newPosition.y = this._followTarget.position.y + this._followOffset.y;
    }

    this._transform.position = newPosition;
  }
}
