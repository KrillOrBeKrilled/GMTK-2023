using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Heroes {
  public class Hero : MonoBehaviour {
    public HeroMovement HeroMovement => this._heroMovement;
    public int Health { get; private set; }

    public UnityEvent<int> OnHealthChanged;
    public UnityEvent<Hero> OnHeroDied;

    public const int MaxHealth = 100;
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
    }

    public void Die() {
      this.HeroHurtEvent.Post(this.gameObject);

      Vector3 heroPos = this.transform.position;
      this.OnHeroDied?.Invoke(this);

      Destroy(this.gameObject);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos)
    {
      this.Die();
    }
  }
}
