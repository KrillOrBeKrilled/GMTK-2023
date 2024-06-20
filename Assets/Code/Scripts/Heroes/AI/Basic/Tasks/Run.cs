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
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Animator _animController;

        /// The hero's regular run movement speed, used to set the current velocity.
        private readonly float _movementSpeed;
        /// Calculates the hero's current velocity, considering speed multiplier and speed penalties applied by traps.
        private float CurrentSpeed => this._movementSpeed * 
                                      (1f - (float)GetData("SpeedPenalty")) * 
                                      (float)GetData("SpeedMultiplier");

        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>.
        /// </summary>
        /// <param name="rigidbody"> Used to set the hero velocity when running. </param>
        /// <param name="spriteRenderer"> Used to flip the sprite if moving backwards. </param>
        /// <param name="animController"> Used to animate the hero when running. </param>
        /// <param name="movementSpeed"> Regular movement velocity to apply to the hero when running. </param>
        public Run(Rigidbody2D rigidbody, SpriteRenderer spriteRenderer, Animator animController, float movementSpeed) {
            this._rigidbody = rigidbody;
            this._spriteRenderer = spriteRenderer;
            this._animController = animController;
            this._movementSpeed = movementSpeed;
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

            float speed = this.CurrentSpeed;
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._spriteRenderer.flipX = speed < 0;

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));

            return NodeStatus.SUCCESS;
        }
    }
}

