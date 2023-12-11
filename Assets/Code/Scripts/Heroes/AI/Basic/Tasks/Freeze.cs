using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Stun
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's Freeze logic.
    /// Freezes hero in place, used when the game has ended.
    /// </summary>
    public class Freeze : Node {
        private readonly Rigidbody2D _rigidbody;
        private readonly Animator _animController;

        public Freeze(Rigidbody2D rigidbody, Animator animController) {
            this._rigidbody = rigidbody;
            this._animController = animController;
        }

        /// <summary>
        /// A task node used to operate the hero AI that governs the hero's Freeze logic.
        /// Freezes hero in place, used when the game has ended.
        /// </summary>
        /// <returns> The <b>success</b> status if "IsFrozen" data is set. </returns>
        internal override NodeStatus Evaluate() {
            if (!(bool)this.GetData("IsFrozen")) {
                this._rigidbody.isKinematic = false;
                return NodeStatus.FAILURE;
            }

            this._rigidbody.isKinematic = true;
            this._rigidbody.velocity = Vector2.zero;
            this._animController.SetFloat((int)this.GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)this.GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));

            return NodeStatus.SUCCESS;
        }
    }
}
