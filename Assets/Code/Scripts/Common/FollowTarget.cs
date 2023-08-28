using UnityEngine;

//*******************************************************************************************
// FollowTarget
//*******************************************************************************************
/// <summary>
/// Follows the position of a specified target with an offset with customizations for
/// the axes that movement should be tracked along.
/// </summary>
public class FollowTarget : MonoBehaviour {
  [SerializeField] private Transform _followTarget;
  [SerializeField] private bool _followTargetX = true;
  [SerializeField] private bool _followTargetY = true;
  [SerializeField] private bool _followTargetZ = true;
  [SerializeField] private Vector3 _followOffset;

  private Transform _transform;

  private void Awake() {
    this._transform = this.transform;
  }

  private void Update() {
    if (this._followTarget is null) {
      this.gameObject.SetActive(false);
      return;
    }

    Vector3 newPosition = this._transform.position;

    if (this._followTargetX) {
      newPosition.x = this._followTarget.position.x + this._followOffset.x;
    }

    if (this._followTargetY) {
      newPosition.y = this._followTarget.position.y + this._followOffset.y;
    }

    if (this._followTargetZ) {
      newPosition.z = this._followTarget.position.z + this._followOffset.z;
    }

    this._transform.position = newPosition;
  }
}
