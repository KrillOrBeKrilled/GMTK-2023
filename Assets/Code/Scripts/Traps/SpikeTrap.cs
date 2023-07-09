using DG.Tweening;
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

    protected override void SetUpTrap()
    {
      // The spikes will pull back into the ground when ready to be detonated
       transform.DOMove(SpawnPosition + Vector3.down * 0.2f, 1f);
    }
    
    protected override void DetonateTrap()
    {
      // Juts out when the hero walks over them
      transform.DOComplete();
      transform.DOMove(SpawnPosition + AnimationOffset, 0.05f);
    }

    protected  override void OnEnteredTrap(Hero hero) {
      if (!IsReady) return;
      
      DetonateTrap();
      hero.TakeDamage(this._damageAmount);
      hero.HeroMovement.SetSpeedPenalty(0.3f);
    }

    protected override void OnExitedTrap(Hero hero) {
      if (!IsReady) return;
      
      hero.HeroMovement.ResetSpeedPenalty();
    }
  }
}
