using System;
using UnityEngine;
using UnityEngine.Events;

public class OutOfBoundsTrigger : MonoBehaviour {
  [SerializeField] private Transform _followTarget;

  public UnityEvent OnPlayerOutOfBounds { get; private set; }
  private Transform _transform;

  private void Awake() {
    this.OnPlayerOutOfBounds = new UnityEvent();
    this._transform = this.transform;
  }

  private void Update() {

    this._transform.position = (Vector2)this._followTarget.position;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    print("Triggered!");
    if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
      return;

    this.OnPlayerOutOfBounds?.Invoke();
  }
}
