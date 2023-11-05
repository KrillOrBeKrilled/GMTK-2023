using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// Run
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's ability to run.
    /// </summary>
    public class Run : Node {
        private readonly Rigidbody2D _rigidbody;
        private readonly Animator _animController;

        private readonly float _movementSpeed;

        public Run(Rigidbody2D rigidbody, Animator animController, float movementSpeed) {
            _rigidbody = rigidbody;
            _animController = animController;

            _movementSpeed = movementSpeed;
        }
        
        /// <summary>
        /// If the hero is able to move, sets the rigidbody velocity to proceed ahead in the horizontal direction while
        /// taking movement speed penalties into account. Updates the animation controller accordingly.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if the hero's moving status is toggled.
        /// The <b>failure</b> status if the hero's moving status is cleared and not allowed.
        /// </returns>
        internal override NodeStatus Evaluate() {
            if (!(bool)GetData("IsMoving")) {
                return NodeStatus.FAILURE;
            }
            
            var speed = this._movementSpeed * (1f - (float)GetData("SpeedPenalty"));
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));

            return NodeStatus.SUCCESS;
        }
    }
}

