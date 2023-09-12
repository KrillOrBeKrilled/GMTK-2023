using KrillOrBeKrilled.Common;
using UnityEngine;

//*******************************************************************************************
// InGroundTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Abstract class extension of <see cref="Trap"/> that sets the player grounded
    /// status to <see langword="false"/> upon contact with the player.
    /// </summary>
    public abstract class InGroundTrap : Trap {
        protected override void OnTriggerStay2D(Collider2D other) {
            ITrapBuilder actor;
            if (other.TryGetComponent(out actor)) {
                // Treat this trap as a non-ground tile if the actor itself collides with this trap
                actor.SetGroundedStatus(false);
            }

            if (IsReady) {
                return;
            }
            
            if (actor is null && !other.CompareTag("Builder Range")) {
                return;
            } else {
                actor = other.GetComponentInParent<ITrapBuilder>();
            }
            
            IsBuilding = actor.CanBuildTrap();
            SoundsController.OnBuild(IsBuilding);
        }
    }
}
