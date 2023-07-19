using UnityEngine;
using UnityEngine.Events;

namespace Traps {
  public class SwingingAxeTrap : Trap {
    [SerializeField] private float _pushbackForce = 1f;
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private Animator _animator;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + LeftSpawnOffset;
    }

    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + RightSpawnOffset;
    }

    protected override void SetUpTrap()
    {
      // Trigger the axe set up animation
      print("Setting up trap");
      _animator.SetTrigger("SetTrap");
    }

    protected override void DetonateTrap()
    {
      // Trigger the axe detonation animation
      print("Trap detonated");
      _animator.SetBool("IsDetonating", true);
    }

    protected override void OnEnteredTrap(Hero hero) {
      if (!IsReady) return;

      DetonateTrap();
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.ThrowHeroBack(0.5f, this._pushbackForce);
    }

    protected override void OnExitedTrap(Hero hero) {

    }

    protected override void OnDetonateTrapAnimationCompete() {
      print("Detonate competed");
      Destroy(this.gameObject);
    }
  }
}
