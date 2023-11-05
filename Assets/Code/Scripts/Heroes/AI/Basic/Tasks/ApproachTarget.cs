using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// ApproachTarget
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A task node used to operate the hero AI that governs the hero's behaviour between
    /// sighting a jump target and reaching it. Moves the hero toward the target and
    /// updates the animation controller.
    /// </summary>
    public class ApproachTarget : Node {
        private readonly Transform _heroTransform;
        private readonly Rigidbody2D _rigidbody;
        private readonly Animator _animController;
        
        private const float _dashSpeed = 8f;

        public ApproachTarget(Transform heroTransform, Rigidbody2D rigidbody, Animator animController) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _animController = animController;
        }
        
        /// <summary>
        /// Moves the hero toward a target jump position at a quickened dash speed and executes associated animations.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if the hero has reached the target position.
        /// The <b>running</b> status if the hero continues to approach the target position.
        /// The <b>failure</b> status if the hero has passed the target position.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var heroPos = _heroTransform.position;
            var targetPos = (Vector3)GetData("JumpLaunchPoint");

            // Ensure the hero is generally around the same position as the launch point before jumping
            if (targetPos.x < heroPos.x && Mathf.Abs(targetPos.y - heroPos.y) < 2f) {
                return NodeStatus.SUCCESS;
            }
            
            // If the hero passes the target point, forget it and switch to another target the next frame
            if (targetPos.x < heroPos.x || Mathf.Abs(targetPos.y - heroPos.y) > 2f) {
                Parent.SetData("JumpLaunchPoint", Vector3.zero);

                return NodeStatus.FAILURE;
            }

            var speed = _dashSpeed * (1f - (float)GetData("SpeedPenalty"));
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            
            return NodeStatus.RUNNING;
        }
    }
}