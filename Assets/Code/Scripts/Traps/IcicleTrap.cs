using System;
using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Managers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        private Rigidbody2D _rigidbody2D;

        private void Awake() {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.gravityScale = 0;
        }

        /// <inheritdoc cref="Trap.SetUpTrap"/>
        /// <remarks> No additional responses at the moment. </remarks>
        protected override void SetUpTrap() {}

        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> Plays the trap detonation animation through the <see cref="Animator"/>. </remarks>
        protected override void DetonateTrap() {
            print("Trap detonating");
            _rigidbody2D.gravityScale = 3;
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
        /// <summary> No responses for exiting the trap. </summary>
        protected override void OnExitedTrap(IDamageable actor) {}

        protected void OnCollisionEnter2D(Collision2D other) {
            if (!other.gameObject.CompareTag("Hero") ||
                !other.gameObject.TryGetComponent(out IDamageable actor)) {
                return;
            }
                
            actor.TakeDamage(_damageAmount);
            TilemapManager.Instance.ResetTrapTiles(TilePositions);
            Destroy(gameObject);
        }
    }
}