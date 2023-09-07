using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Heroes {
  public class Hero : MonoBehaviour {
    public HeroMovement HeroMovement => this._heroMovement;
    public int Health { get; private set; }

    public float MapPosition => (this.transform.position.x - this._levelStart.position.x) / (this._levelEnd.position.x - this._levelStart.position.x);

    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<Hero> OnHeroDied;

    public const int MaxHealth = 100;
    public AK.Wwise.Event HeroHurtEvent;

    private HeroMovement _heroMovement;
    private Animator _animator;
    private static readonly int SpawningKey = Animator.StringToHash("spawning");

    private Transform _levelStart;
    private Transform _levelEnd;

    private const int CoinsEarnedOnDeath = 2;

    public void Initialize(Transform levelStart, Transform levelEnd) {
      this._levelStart = levelStart;
      this._levelEnd = levelEnd;
    }


    public void TakeDamage(int amount) {
      this.Health -= amount;

      this.HeroHurtEvent.Post(this.gameObject);
      this.OnHealthChanged?.Invoke(this.Health);

      if (this.Health <= 0) {
        this.Die();
      }
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

      this.Health = MaxHealth;
    }

    public void Die() {
      this.HeroHurtEvent.Post(this.gameObject);
      CoinManager.Instance.EarnCoins(CoinsEarnedOnDeath);

      this.OnHeroDied?.Invoke(this);
      Destroy(this.gameObject);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos)
    {
      this.Die();
    }
  }
}
