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
            var pitQueue = (Queue<(Vector3, Vector3)>)GetData("PitQueue");

            // // TODO: If the hero gets dangerously close to a pit that hasn't been fully mapped out, emergency push
            // // to the PitQueue with unfinished data -> Update "AbortPitTracking"
            // // Pushing the pit track data should get the LookForObstacle script to update the next jump point right 
            // // away, so DecideJumpForce would know
            // if (targetPos.x - heroPos.x < 1f) {
            //     Parent.SetData("AbortPitTracking", true);
            // }

            if (targetPos.x - heroPos.x < 0.5f) {
                // It's jumping time!
                return NodeStatus.SUCCESS;
            }
            
            if (targetPos.x < heroPos.x) {
                // For any reason, the hero walked past the jump point
                Debug.Log("Dashed past the target!");
                Debug.Log("Target: " + targetPos);
                Debug.Log("Hero transform: " + heroPos);
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