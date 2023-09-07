using Heroes;
using Managers;
using UnityEngine;

//*******************************************************************************************
// SwingingAxeTrap
//*******************************************************************************************
namespace Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a permanent 2x3 grounded area and
    /// damages and throws back the hero through <see cref="HeroMovement"/>. 
    /// </summary>
    public class SwingingAxeTrap : Trap {
        [Tooltip("Used to scale the knock back force applied to the HeroMovement upon detonation.")]
        [SerializeField] private float _pushbackForce = 1f;
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount = 10;
        [SerializeField] private Animator _animator;

        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> Plays the trap readying animation through the <see cref="Animator"/>. </remarks>
        protected override void SetUpTrap() {
            // Trigger the axe set up animation
            print("Setting up trap");
            _animator.SetTrigger("SetTrap");
        }

        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Plays the trap detonation animation through the <see cref="Animator"/>. </remarks>
        protected override void DetonateTrap() {
            // Trigger the axe detonation animation
            print("Trap detonated");
            _animator.SetBool("IsDetonating", true);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap applies a knock back force to the <see cref="HeroMovement"/> and flat damage to the
        /// <see cref="Hero"/>.
        /// </summary>
        protected override void OnEnteredTrap(Hero hero) {
            if (!IsReady) 
                return;

            DetonateTrap();
            hero.TakeDamage(this._damageAmount);
            hero.HeroMovement.ThrowHeroBack(0.5f, this._pushbackForce);
        }

        protected override void OnExitedTrap(Hero hero) {

        }

        protected override void OnDetonateTrapAnimationCompete() {
            print("Detonate competed");
            TilemapManager.Instance.ResetTrapTiles(TilePositions);
            Destroy(this.gameObject);
        }
    }
}
