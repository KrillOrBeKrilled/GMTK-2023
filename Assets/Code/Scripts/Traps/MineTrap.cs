using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }
    protected override void OnEnteredTrap(Hero hero) {
      print("Hit Mine, BOOM!");
    }
    protected override void OnExitedTrap(Hero hero) {
      throw new System.NotImplementedException();
    }
  }
}
