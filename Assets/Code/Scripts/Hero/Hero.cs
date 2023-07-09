using System.Collections;
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
  public float RespawnTime = 3;
  public AK.Wwise.Event HeroHurtEvent;
  
  private HeroMovement _heroMovement;
  private Animator _animator;
  private static readonly int SpawningKey = Animator.StringToHash("spawning");


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
    this.TryGetComponent(out this._animator);
    
    ResetHero();
  }

  private void Die()
  {
    Lives--;
    HeroHurtEvent.Post(gameObject);
    this.OnHeroDied?.Invoke();

    if (Lives == 0)
    {
      OnGameOver.Invoke();
      return;
    }

    StartCoroutine(Respawn());
  }

  private IEnumerator Respawn()
  {
    transform.position = RespawnPoint.transform.position;
    _animator.SetBool(SpawningKey, true);
    _heroMovement.ToggleMoving(false);

    // Gradually fill the health bar over the respawn time
    float timePassed = 0;
    while (timePassed < RespawnTime)
    {
      timePassed += Time.deltaTime;

      Health = (int) Mathf.Lerp(0f, MaxHealth, timePassed / RespawnTime);
      OnHealthChanged.Invoke(Health);

      yield return new WaitForEndOfFrame();
    }
    
    _heroMovement.ToggleMoving(true);
    _animator.SetBool(SpawningKey, false);
  }

  public void SetRespawnPoint(HeroRespawnPoint respawnPoint)
  {
    RespawnPoint = respawnPoint;
  }

  private void ResetHero()
  {
    Health = MaxHealth;
    Lives = MaxLives;
    OnHealthChanged.Invoke(Health);
    OnHeroReset.Invoke();
  }
}
