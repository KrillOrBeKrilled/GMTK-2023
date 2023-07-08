using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour {
  public HeroMovement HeroMovement => this._heroMovement;

  public UnityEvent<int> OnHealthChanged;
  public UnityEvent OnHeroDied;

  private int _health;
  private HeroMovement _heroMovement;

  public void TakeDamage(int amount) {
    this._health -= amount;

    if (this._health <= 0) {
      this.Die();
    }

    this.OnHealthChanged?.Invoke(this._health);
  }

  private void Awake() {
    this._health = 100;
    this.OnHeroDied = new UnityEvent();
    this.TryGetComponent(out this._heroMovement);
  }

  private void Die() {
    this.OnHeroDied?.Invoke();
    Destroy(this.gameObject);
  }
}
