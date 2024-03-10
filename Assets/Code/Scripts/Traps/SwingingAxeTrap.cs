using KrillOrBeKrilled.Traps.Interfaces;
using UnityEngine;

//*******************************************************************************************
// SwingingAxeTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a permanent 2x3 grounded area and
    /// damages and throws back the hero. 
    /// </summary>
    public class SwingingAxeTrap : Trap {
        [Tooltip("Used to scale the knock back force applied to the HeroMovement upon detonation.")]
        [SerializeField] private float _pushbackForce = 1f;
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount = 10;
        [SerializeField] private Animator _animator;

        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Plays the trap detonation animation through the <see cref="Animator"/>. </remarks>
        protected override void DetonateTrap() {
            // Trigger the axe detonation animation
            this._animator.SetBool("IsDetonating", true);
        }
        
        /// <summary>
        /// Destroys the trap and frees the trap tile grid units through the <see cref="TilemapManager"/>.
        /// </summary>
        /// <remarks> Triggered by the detonate_axe animation event. </remarks>
        protected override void OnDetonateTrapAnimationCompete() {
            Destroy(this.gameObject);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// Applies a knock back force and flat damage to the <see cref="ITrapDamageable"/> actor.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnEnteredTrap(ITrapDamageable actor) {
            DetonateTrap();
            actor.TakeTrapDamage(this._damageAmount, this);
            actor.TrapThrowActorBack(0.5f, this._pushbackForce);
        }

        protected override void OnExitedTrap(ITrapDamageable actor) {}
        
        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> Plays the trap readying animation through the <see cref="Animator"/>. </remarks>
        protected override void SetUpTrap() {
            // Trigger the axe set up animation
            this._animator.SetTrigger("SetTrap");
        }
        
        #endregion
    }
}
