using KrillOrBeKrilled.Traps.Interfaces;
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
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        protected override void OnTriggerStay2D(Collider2D other) {
            // if (other.TryGetComponent(out ITrapBuilder actor)) {
            //     // Treat this trap as a non-ground tile if the actor itself collides with this trap
            //     actor.SetGroundedStatus(false);
            // }
        }
        
        #endregion
    }
}
