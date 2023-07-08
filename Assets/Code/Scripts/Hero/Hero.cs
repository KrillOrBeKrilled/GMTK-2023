using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour {
  public HeroMovement HeroMovement => this._heroMovement;
  public int Health { get; private set; }

  public UnityEvent<int> OnHealthChanged;
  public UnityEvent OnHeroDied;
  public const int MaxHealth = 100;

  private HeroMovement _heroMovement;

  public void TakeDamage(int amount) {
    this.Health -= amount;

    if (this.Health <= 0) {
      this.Die();
    }

    this.OnHealthChanged?.Invoke(this.Health);
  }

  private void Awake() {
    this.Health = MaxHealth;
    this.OnHeroDied = new UnityEvent();
    this.TryGetComponent(out this._heroMovement);
  }

  private void Die() {
    this.OnHeroDied?.Invoke();
    Destroy(this.gameObject);
  }
}
