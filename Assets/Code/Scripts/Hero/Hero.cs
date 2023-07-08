using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour {
  public int Health { get; private set; }

  public UnityEvent OnHeroDied;

  public void TakeDamage(int amount) {
    this.Health -= amount;
    if (this.Health <= 0) {
      this.Die();
    }
  }

  private void Awake() {
    this.Health = 100;
    this.OnHeroDied = new UnityEvent();
  }

  private void Die() {
    this.OnHeroDied?.Invoke();
    Destroy(this.gameObject);
  }
}
