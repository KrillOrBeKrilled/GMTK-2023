using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected override void OnEnteredTrap(Hero hero) {

    }

    protected override void OnExitedTrap(Hero hero) {
      
    }
  }
}
