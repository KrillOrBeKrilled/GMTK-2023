using UnityEngine;

namespace Traps {
  public class MineTrap : Trap {

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }
    protected override void Detonate(Collider2D target) {
      print("Hit Mine, BOOM!");
    }
  }
}
