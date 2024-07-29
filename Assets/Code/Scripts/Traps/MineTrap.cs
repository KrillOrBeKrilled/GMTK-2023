using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Traps.Interfaces;
using UnityEngine;

//*******************************************************************************************
// MineTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that damages and throws back the hero.
    /// </summary>
    public class MineTrap : Trap {
        [Tooltip("Used to scale the knock back force applied to the HeroMovement upon detonation.")]
        [SerializeField] private float _explosionForce = 2f;
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount = 10;
        
        //========================================
        // Public Methods
        //======================================== 
        
        public override TrapType GetTrapType() {
            return TrapType.Mine;
        }
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        protected override void DetonateTrap() {}

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// Applies a knock back force and flat damage to the <see cref="ITrapDamageable"/> actor before
        /// destroying itself.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnEnteredTrap(ITrapDamageable actor) {
            actor.TakeTrapDamage(this._damageAmount, this);
            actor.TrapThrowActorBack(2f, this._explosionForce);
            Destroy(this.gameObject);
        }

        protected override void OnExitedTrap(ITrapDamageable actor) {}
        
        protected override void SetUpTrap() {}
        
        #endregion
    }
}
