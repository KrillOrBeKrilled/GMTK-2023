using Heroes;
using UnityEngine;

//*******************************************************************************************
// MineTrap
//*******************************************************************************************
namespace Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that damages and throws back the hero through
    /// <see cref="HeroMovement"/>.
    /// </summary>
    public class MineTrap : Trap {
        [Tooltip("Used to scale the knock back force applied to the HeroMovement upon detonation.")]
        [SerializeField] private float _explosionForce = 2f;
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount = 10;
        
        protected override void SetUpTrap() {

        }
        
        protected override void DetonateTrap() {

        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap applies a knock back force to the <see cref="HeroMovement"/> and flat damage to the
        /// <see cref="Hero"/> before destroying itself.
        /// </summary>
        protected override void OnEnteredTrap(Hero hero) {
            if (!IsReady) 
                return;
          
            hero.TakeDamage(this._damageAmount);
            hero.HeroMovement.ThrowHeroBack(2f, this._explosionForce);
            Destroy(this.gameObject);
        }

        protected override void OnExitedTrap(Hero hero) {

        }
    }
}
