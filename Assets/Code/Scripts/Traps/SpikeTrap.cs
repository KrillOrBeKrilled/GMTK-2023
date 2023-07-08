using UnityEngine;

namespace Traps {
  public class SpikeTrap : Trap {

    public override void AdjustSpawnPoint() {
      throw new System.NotImplementedException();
    }

    protected  override void Detonate(Collider2D target) {
      print("Hit spikes!");
    }
  }
}
