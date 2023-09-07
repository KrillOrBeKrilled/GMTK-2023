using Player;
using UnityEngine;

//*******************************************************************************************
// InGroundTrap
//*******************************************************************************************
namespace Traps {
    /// <summary>
    /// Abstract class extension of <see cref="Trap"/> that sets the player grounded
    /// status to <see langword="false"/> upon contact with the player.
    /// </summary>
    public abstract class InGroundTrap : Trap {
        protected override void OnTriggerStay2D(Collider2D other) {
            if (!other.CompareTag("Player")) 
                return;
            
            // If the player itself collides with this trap, treat this trap as a non-ground tile
            if (other.TryGetComponent(out PlayerController playerController)) {
                playerController.SetGroundedStatus(false);
            }
            else {
                playerController = other.GetComponentInParent<PlayerController>();
            }
            
            var playerState = playerController.GetPlayerState();
            
            // If the trap is not ready and the player is idle, start building the trap
            if (IsReady || playerState is not IdleState) 
                return;

            IsBuilding = true;
            SoundsController.OnStartBuild();
        }
    }
}
