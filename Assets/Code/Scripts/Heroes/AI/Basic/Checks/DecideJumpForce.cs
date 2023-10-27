using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// DecideJumpForce
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class DecideJumpForce : Node {
        private Transform _heroTransform;
        private float _maxJumpForce, _minJumpForce; 
        private float _jumpAngle;
        private float _jumpErrorMargin;
        
        private float _dashSpeed;
        
        public DecideJumpForce(Transform heroTransform, float minJumpForce, float maxJumpForce, float jumpAngle, float jumpErrorMargin, float dashSpeed) {
            _heroTransform = heroTransform;
            _maxJumpForce = maxJumpForce;
            _minJumpForce = minJumpForce;
            _jumpAngle = jumpAngle;
            _jumpErrorMargin = jumpErrorMargin;

            _dashSpeed = dashSpeed;
        }
        
        internal override NodeStatus Evaluate() {
            var targetPos = (Vector3)GetData("JumpLandPoint");
            var launchPos = (Vector3)GetData("JumpLaunchPoint");

            // If no jump endpoint has been defined, the hero will jump with maximum force
            if (targetPos == Vector3.zero) {
                Parent.SetData("JumpForce", _maxJumpForce);
                return NodeStatus.SUCCESS;
            } else {
                // TODO: There's always some margin of error in jumping
                // targetPos.x = Random.Range(targetPos.x - _jumpErrorMargin, targetPos.x + _jumpErrorMargin);
            }

            var heroPos = _heroTransform.position;
            var initialMinHeight = (Vector3)GetData("JumpInitialMinHeight");
            var totalMinHeight = (Vector3)GetData("JumpApexMinHeight");
            
            var jumpAngle = _jumpAngle * Mathf.Deg2Rad;
            var gravity = Physics.gravity.magnitude;
            
            // Formula retrieved by: 
            // 1. Time in the air in the y-direction equation
            // => (target y) = (initial y) + (v0 * sin(jumpAngle) * t) - 1/2gt^2, where v0 = initial velocity
            // 2. Solve for the time in the air in the x-direction -> _dashSpeed is the velocity in the x-direction
            // => t = (distance) / (_dashSpeed)
            // 3. Substitute #2 back into #1 and solve for v0
            
            var distance = targetPos.x - heroPos.x;
            var v0 = (targetPos.y - heroPos.y + (0.5f * gravity * Mathf.Pow(distance / _dashSpeed, 2))) /
                     (Mathf.Sin(jumpAngle) * (distance / _dashSpeed));
            
            // Adjust the velocity depending on the elevation difference
            v0 -= (targetPos.y - heroPos.y) * 0.55f;
            
            // Abort jump if the jumpForce is negligible
            if (v0 < 1f) {
                Parent.Parent.Parent.SetData("CanJump", false);
                Parent.SetData("JumpLaunchPoint", Vector3.zero);
                Parent.SetData("JumpLandPoint", Vector3.zero);
                Parent.SetData("JumpInitialMinHeight", Vector3.zero);
                Parent.SetData("JumpApexMinHeight", Vector3.zero);

                return NodeStatus.FAILURE;
            }
            
            Parent.SetData("JumpForce", Mathf.Clamp(v0, _minJumpForce, _maxJumpForce));
            
            // Note that we will have to compare the maximum jumpForce between the initial velocity needed to clear the
            // tallest trap and the target point to reach

            return NodeStatus.SUCCESS;
        }
    }
}