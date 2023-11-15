using System.Collections.Generic;
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
        
        /// The hero's regular run movement speed, used to reset the speed blending.
        private readonly float _movementSpeed;
        /// The duration of time needed to blend the velocities between regular movement and dash movement.
        private readonly float _speedBlendDuration;
        /// The dash movement velocity.
        private const float _dashSpeed = 7f;
        private float _currentSpeed, _t;

        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>.
        /// </summary>
        /// <param name="heroTransform"> Provides the hero position to check when the jump waypoint is reached. </param>
        /// <param name="rigidbody"> Used to set the hero velocity when dashing towards the target. </param>
        /// <param name="animController"> Used to animate the hero during a dash. </param>
        /// <param name="movementSpeed"> The hero's normal movement speed. </param>
        /// <param name="speedBlendDuration">
        /// The duration of time needed to blend the velocities between regular movement and dash movement.
        /// </param>
        public ApproachTarget(Transform heroTransform, Rigidbody2D rigidbody, Animator animController, 
            float movementSpeed, float speedBlendDuration) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _animController = animController;
            
            _movementSpeed = movementSpeed;
            _speedBlendDuration = speedBlendDuration;
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

            var isHeroPastLaunch = targetPos.x < heroPos.x;
            var isHeroOnPlatform = Mathf.Abs(targetPos.y - heroPos.y) < 2f;

            switch (isHeroPastLaunch) {
                // Ensure the hero is generally around the same position as the launch point before jumping
                case true when isHeroOnPlatform:
                    _currentSpeed = _movementSpeed;
                    _t = 0;
                    return NodeStatus.SUCCESS;
                // If the hero passes the target point, forget it and switch to another target the next frame
                case true:
                    var pitList = (List<(Vector3, Vector3)>)GetData("PitList");
                    if (pitList.Count > 0) {
                        pitList.RemoveAt(0);
                    }
                    
                    Parent.SetData("JumpLaunchPoint", Vector3.zero);

                    return NodeStatus.FAILURE;
            }

            
            if (_t < 1) {
                _currentSpeed = Mathf.SmoothStep(_currentSpeed, _dashSpeed, _t / _speedBlendDuration);
                _t += Time.deltaTime;
            }

            var speed = _currentSpeed * (1f - (float)GetData("SpeedPenalty"));
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            
            return NodeStatus.RUNNING;
        }
    }
}