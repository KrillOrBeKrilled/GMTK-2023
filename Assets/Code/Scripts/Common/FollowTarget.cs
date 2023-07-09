using UnityEngine;

public class FollowTarget : MonoBehaviour {
  [SerializeField] private Transform _followTarget;
  [SerializeField] private Vector2 _followOffset;

  private Transform _transform;

  private void Awake() {
    this._transform = this.transform;
  }

  private void Update() {
    if (this._followTarget == null)
      return;

    this._transform.position = (Vector2)this._followTarget.position + this._followOffset;
  }
}
