using UnityEngine;

namespace Traps {
  public class SpikeTrap : Trap {
    [SerializeField] private int _damageAmount;

    public override Vector3 GetLeftSpawnPoint(Vector3 origin)
    {
      return origin + LeftSpawnOffset;
    }
        
    public override Vector3 GetRightSpawnPoint(Vector3 origin)
    {
      return origin + RightSpawnOffset;
    }

    protected  override void OnEnteredTrap(Hero hero) {
      if (!IsReady) return;
      
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.SetSpeedPenalty(0.3f);
    }

    protected override void OnExitedTrap(Hero hero) {
      if (!IsReady) return;
      
      hero.HeroMovement.ResetSpeedPenalty();
    }
  }
}
