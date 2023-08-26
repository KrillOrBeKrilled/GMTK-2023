using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Heroes {
  public class Hero : MonoBehaviour {
    public HeroMovement HeroMovement => this._heroMovement;
    public int Health { get; private set; }
    public int Lives { get; private set; }

    public HeroRespawnPoint RespawnPoint;

    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<int, float, float, float> OnHeroDied;
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
      CoinManager.Instance.EarnCoins(1);

      if (this.Health <= 0) {
        this.Die();
      }

      this.HeroHurtEvent.Post(this.gameObject);
      this.OnHealthChanged?.Invoke(this.Health);
    }

    public void StartRunning()
    {
      this.StopAllCoroutines();
      this._heroMovement.ToggleMoving(true);
    }

    public void EnterLevel()
    {
      this.StartCoroutine(this.EnterLevelAnimation());
    }

    private IEnumerator EnterLevelAnimation()
    {
      this._heroMovement.ToggleMoving(true);

      yield return new WaitForSeconds(2f);

      this._heroMovement.ToggleMoving(false);
    }

    private void Awake() {
      this.TryGetComponent(out this._heroMovement);
      this.TryGetComponent(out this._animator);
      this.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

      this.ResetHero();
    }

    public void Die()
    {
      this.Lives--;
      this.HeroHurtEvent.Post(this.gameObject);

      var heroPos = this.transform.position;
      this.OnHeroDied?.Invoke(this.Lives, heroPos.x, heroPos.y, heroPos.z);

      if (this.Lives == 0)
      {
        this.OnGameOver.Invoke();
        Destroy(this.gameObject);
        return;
      }

      this.StartCoroutine(this.Respawn());
    }

    public void OnHeroIsStuck(float xPos, float yPos, float zPos)
    {
      this.Die();
    }

    private IEnumerator Respawn()
    {
      this.transform.position = this.RespawnPoint.transform.position;
      this._animator.SetBool(SpawningKey, true);
      this._heroMovement.ToggleMoving(false);

      // Gradually fill the health bar over the respawn time
      float timePassed = 0;
      while (timePassed < this.RespawnTime)
      {
        timePassed += Time.deltaTime;

        this.Health = (int) Mathf.Lerp(0f, MaxHealth, timePassed / this.RespawnTime);
        this.OnHealthChanged.Invoke(this.Health);

        yield return new WaitForEndOfFrame();
      }

      this._heroMovement.ToggleMoving(true);
      this._animator.SetBool(SpawningKey, false);
    }

    public void SetRespawnPoint(HeroRespawnPoint respawnPoint)
    {
      this.RespawnPoint = respawnPoint;
    }

    public void ResetHero()
    {
      this._heroMovement.ToggleMoving(false);
      this.Health = MaxHealth;
      this.Lives = MaxLives;
      this.OnHealthChanged.Invoke(this.Health);
      this.OnHeroReset.Invoke();
    }
  }
}
