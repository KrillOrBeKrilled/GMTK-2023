using UnityEngine;

namespace Traps {
  public class SwingingAxeTrap : Trap {
    [SerializeField] private float _pushbackForce = 1f;
    [SerializeField] private int _damageAmount = 10;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + LeftSpawnOffset;
    }
        
    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + RightSpawnOffset;
    }

    protected override void OnEnteredTrap(Hero hero) {
      if (!IsReady) return;
      
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.ThrowHeroBack(0.5f, this._pushbackForce);
      Destroy(this.gameObject);
    }

    protected override void OnExitedTrap(Hero hero) {

    }
  }
}
