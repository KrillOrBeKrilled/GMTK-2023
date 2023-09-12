using KrillOrBeKrilled.Common;
using DG.Tweening;
using UnityEngine;

//*******************************************************************************************
// SpikeTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="Trap"/> that fills a permanent 3x1 grounded area and
    /// damages the <see cref="Hero"/> with a speed penalty. 
    /// </summary>
    public class SpikeTrap : Trap {
        [Tooltip("The damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;

        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> Pulls the spike back into the ground in wait of the hero to walk over them. </remarks>
        protected override void SetUpTrap() {
            transform.DOMove(SpawnPosition + Vector3.down * 0.2f, 1f);
        }
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Immediately juts the spikes out when the hero walks over them. </remarks>
        protected override void DetonateTrap() {
            transform.DOComplete();
            transform.DOMove(SpawnPosition + AnimationOffset, 0.05f);
        }

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap applies an 30% speed reduction to the <see cref="HeroMovement"/> and flat damage to the
        /// <see cref="Hero"/>.
        /// </summary>
        protected  override void OnEnteredTrap(IDamageable actor) {
            if (!IsReady) 
                return;

            DetonateTrap();
            actor.TakeDamage(this._damageAmount);
            actor.ApplySpeedPenalty(0.3f);
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary> Resets the speed reduction through <see cref="HeroMovement"/>. </summary>
        protected override void OnExitedTrap(IDamageable actor) {
            if (!IsReady) 
                return;

            actor.ResetSpeedPenalty();
        }
    }
}
