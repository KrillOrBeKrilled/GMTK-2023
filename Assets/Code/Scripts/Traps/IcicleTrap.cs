using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Managers;
using UnityEngine;

//*******************************************************************************************
// IcicleTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a 2x2 area on the ceiling
    /// and damages the <see cref="Hero"/> when the hero is underneath it.
    /// </summary>
    public class IcicleTrap : Trap {
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;
        [SerializeField] private Animator _animator;
        
        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> No additional responses at the moment. </remarks>
        protected override void SetUpTrap() {}

        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Plays the trap detonation animation through the <see cref="Animator"/>. </remarks>
        protected override void DetonateTrap() {
            print("Trap detonating");
            _animator.SetBool("IsDetonating", true);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap does a flat damage to the <see cref="Hero"/>. No special effects at the moment.
        /// </summary>
        protected override void OnEnteredTrap(IDamageable actor) {
            if (!IsReady) {
                return;
            }

            DetonateTrap();
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary> Damage the <see cref="Hero"/> when the hero leaves the trap, because
        /// this is in sync with the animation. </summary>
        protected override void OnExitedTrap(IDamageable actor) {
            actor.TakeDamage(_damageAmount);
        }

        protected override void OnDetonateTrapAnimationCompete() {
            print("Animation Complete");
            TilemapManager.Instance.ResetTrapTiles(TilePositions);
            Destroy(gameObject);
        }
    }
}