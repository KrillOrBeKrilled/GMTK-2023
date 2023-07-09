using UnityEngine;
using UnityEngine.Events;

public class OutOfBoundsTrigger : MonoBehaviour {
  public UnityEvent OnPlayerOutOfBounds { get; private set; }
  private bool _isOn;

  private void Awake() {
    this.OnPlayerOutOfBounds = new UnityEvent();
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.gameObject.layer != LayerMask.NameToLayer("Player") || !_isOn)
      return;

    this.OnPlayerOutOfBounds?.Invoke();
  }

  public void ToggleBounds(bool value)
  {
    _isOn = value;
  }
}
