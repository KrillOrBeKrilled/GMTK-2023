using Managers;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Heroes {
  public class Hero : MonoBehaviour {
    public HeroMovement HeroMovement => this._heroMovement;
    public int Health { get; private set; }
    public HeroData.HeroType Type { get; private set; }

    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<Hero> OnHeroDied;

    public AK.Wwise.Event HeroHurtEvent;

    private HeroMovement _heroMovement;
    private Animator _animator;
    private static readonly int SpawningKey = Animator.StringToHash("spawning");

    private const int CoinsEarnedOnDeath = 2;

    public void Initialize(HeroData heroData) {
      this.Health = heroData.Health;
      this.Type = heroData.Type;
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

    public void Die() {
      this.HeroHurtEvent.Post(this.gameObject);
      CoinManager.Instance.EarnCoins(CoinsEarnedOnDeath);

      this.OnHeroDied?.Invoke(this);
      Destroy(this.gameObject);
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
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos)
    {
      this.Die();
    }
  }
}
