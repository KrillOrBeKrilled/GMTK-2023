using UnityEngine;
using UnityEngine.Events;

public class OutOfBoundsTrigger : MonoBehaviour {
  public UnityEvent<float, float, float> OnPlayerOutOfBounds { get; private set; }
  private bool _isOn;

  private void Awake() {
    this.OnPlayerOutOfBounds = new UnityEvent<float, float, float>();
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.gameObject.layer != LayerMask.NameToLayer("Player") || !_isOn)
      return;

    var playerPos = other.transform.position;
    this.OnPlayerOutOfBounds?.Invoke(playerPos.x, playerPos.y, playerPos.z);
  }

  public void ToggleBounds(bool value)
  {
    _isOn = value;
  }
}
