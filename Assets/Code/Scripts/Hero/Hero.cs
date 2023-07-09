using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour {
  public HeroMovement HeroMovement => this._heroMovement;
  public int Health { get; private set; }
  public int Lives { get; private set; }

  public HeroRespawnPoint RespawnPoint;

  public UnityEvent<int> OnHealthChanged;
  public UnityEvent OnHeroDied;
  public UnityEvent OnGameOver;
  public UnityEvent OnHeroReset;
  public const int MaxHealth = 100;
  public const int MaxLives = 3;
  public AK.Wwise.Event HeroHurtEvent;
  
  private HeroMovement _heroMovement;


  public void TakeDamage(int amount) {
    this.Health -= amount;

    if (this.Health <= 0) {
      this.Die();
    }

    HeroHurtEvent.Post(gameObject);
    this.OnHealthChanged?.Invoke(this.Health);
  }

  private void Awake() {
    this.TryGetComponent(out this._heroMovement);
    
    ResetHero();
  }

  private void Die()
  {
    Lives--;
    HeroHurtEvent.Post(gameObject);
    this.OnHeroDied?.Invoke();
    Respawn();

    if (Lives == 0)
    {
      OnGameOver.Invoke();
    }
  }

  public void Respawn()
  {
    transform.position = RespawnPoint.transform.position;
  }

  public void SetRespawnPoint(HeroRespawnPoint respawnPoint)
  {
    RespawnPoint = respawnPoint;
  }

  private void ResetHero()
  {
    Health = MaxHealth;
    Lives = MaxLives;
    OnHeroReset.Invoke();
  }
}
