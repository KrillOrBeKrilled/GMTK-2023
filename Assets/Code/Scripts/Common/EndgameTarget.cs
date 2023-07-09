using UnityEngine;
using UnityEngine.Events;

public class EndgameTarget : MonoBehaviour {
  public UnityEvent OnHeroReachedEndgameTarget { get; private set; }

  private void Awake() {
    this.OnHeroReachedEndgameTarget = new UnityEvent();
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.layer != LayerMask.NameToLayer("Hero"))
      return;

    this.OnHeroReachedEndgameTarget?.Invoke();
  }
}
