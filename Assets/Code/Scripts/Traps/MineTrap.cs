using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }
    protected override void TriggerTrap(Hero hero) {
      print("Hit Mine, BOOM!");
    }
  }
}
