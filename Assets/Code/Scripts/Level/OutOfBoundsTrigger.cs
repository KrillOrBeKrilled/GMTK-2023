using System;
using UnityEngine;
using UnityEngine.Events;

public class OutOfBoundsTrigger : MonoBehaviour {
  [SerializeField] private Transform _followTarget;

  public UnityEvent OnPlayerOutOfBounds { get; private set; }
  private Transform _transform;
  private bool _isOn;

  private void Awake() {
    this.OnPlayerOutOfBounds = new UnityEvent();
    this._transform = this.transform;
  }

  private void Update() {
    this._transform.position = (Vector2)this._followTarget.position;
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
