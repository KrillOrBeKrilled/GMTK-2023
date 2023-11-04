using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// ApproachTarget
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class ApproachTarget : Node {
        private Transform _heroTransform;
        private Rigidbody2D _rigidbody;
        private Animator _animController;

        // The hero will dash to the jump target, but this can also be the same speed as normal movement
        private float _dashSpeed;
        
        public ApproachTarget(Transform heroTransform, Rigidbody2D rigidbody, Animator animController, float dashSpeed) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _animController = animController;

            _dashSpeed = dashSpeed;
        }
        
        internal override NodeStatus Evaluate() {
            var heroPos = _heroTransform.position;
            var targetPos = (Vector3)GetData("JumpLaunchPoint");
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");

            // Ensure the hero is generally around the same position as the launch point
            if (targetPos.x < heroPos.x && Mathf.Abs(targetPos.y - heroPos.y) < 2f) {
                // It's jumping time!
                // Debug.Log("Prepare to jump!");
                // Debug.Log("Target position: " + );
                // Debug.Log("Prepare to jump!");
                return NodeStatus.SUCCESS;
            }
            
            // If the hero passes the target point, forget it and switch to another target the next frame
            if (targetPos.x < heroPos.x || Mathf.Abs(targetPos.y - heroPos.y) > 2f) {
                Parent.SetData("JumpLaunchPoint", Vector3.zero);
                Parent.SetData("JumpLandPoint", Vector3.zero);
                Parent.SetData("JumpInitialMinHeight", Vector3.zero);
                Parent.SetData("JumpApexMinHeight", Vector3.zero);

                return NodeStatus.FAILURE;
            }

            var speed = this._dashSpeed * (1f - (float)GetData("SpeedPenalty"));
            this._rigidbody.velocity = new Vector2(speed, this._rigidbody.velocity.y);

            this._animController.SetFloat((int)GetData("XSpeedKey"), Mathf.Abs(this._rigidbody.velocity.x));
            this._animController.SetFloat((int)GetData("YSpeedKey"), Mathf.Abs(this._rigidbody.velocity.y));
            
            return NodeStatus.RUNNING;
        }
    }
}