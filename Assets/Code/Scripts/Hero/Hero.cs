using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour {
  public HeroMovement HeroMovement => this._heroMovement;
  public int Health { get; private set; }
  public int Lives { get; private set; }

  public UnityEvent<int> OnHealthChanged;
  public UnityEvent OnHeroDied;
  public UnityEvent OnHeroReset;
  public const int MaxHealth = 100;
  public const int MaxLives = 3;

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
    this.TryGetComponent(out this._heroMovement);
  }

  private void Die()
  {
    Lives--;
    this.OnHeroDied?.Invoke();
    Destroy(this.gameObject);
  }

  private void ResetHero()
  {
    Health = MaxHealth;
    Lives = MaxLives;
    OnHeroReset.Invoke();
  }
}
