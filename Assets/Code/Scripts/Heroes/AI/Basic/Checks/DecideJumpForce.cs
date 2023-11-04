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
        
        private float _dashSpeed;
        
        public DecideJumpForce(Transform heroTransform, float minJumpForce, float maxJumpForce, float jumpAngle, float dashSpeed) {
            _heroTransform = heroTransform;
            _maxJumpForce = maxJumpForce;
            _minJumpForce = minJumpForce;
            _jumpAngle = jumpAngle;

            _dashSpeed = dashSpeed;
        }
        
        internal override NodeStatus Evaluate() {
            var targetPos = (Vector3)GetData("JumpLandPoint");
            var launchPos = (Vector3)GetData("JumpLaunchPoint");
            
            // Debug.Log("Jumping to: " + targetPos);
            // Debug.Log("From: " + launchPos);

            // If no jump endpoint has been defined, the hero will jump with maximum force
            if (targetPos == Vector3.zero) {
                Debug.Log("No target jump force registered!");
                Parent.SetData("JumpAngle", new Vector3(_dashSpeed, 0, 0));
                return NodeStatus.SUCCESS;
            }

            var heroPos = _heroTransform.position;
            var distance = targetPos.x - launchPos.x;

            // TODO: Have different formulas for jumping to higher heights and to lower heights
            Vector3 initialVelocity;
            var gravity = Physics.gravity.magnitude * 3f;
            var elevationDifference = targetPos.y - launchPos.y;

            if (elevationDifference > 1f && distance < 2.5f || distance > 5.5f) {
                var positiveElevationDifference = Mathf.Abs(elevationDifference);
                var jumpApex = targetPos.y < launchPos.y
                    ? new Vector3(
                        (distance - 6.35f) * 0.34f + 1.5f,
                        launchPos.y + (distance - 6.35f) * 0.02f + 1 / positiveElevationDifference * ((distance - 6.35f) * 0.5f + 1.5f),
                        0
                    )
                    : new Vector3(
                        distance * 0.5f,
                        positiveElevationDifference + positiveElevationDifference * 0.03f + distance * 0.3f + 2.5f,
                        0
                    );
                
                // Debug.Log("ElevationDifference: " + elevationDifference);
                // Debug.Log("Distance: " + distance);
                // Debug.Log("Jump apex: " + jumpApex);

                var jumpAngle = Mathf.Asin(jumpApex.normalized.y);

                // Use Toricelli's formula to get initial velocity to reach the apex
                var v0 = Mathf.Sqrt(2 * gravity * Mathf.Max(0, jumpApex.y));
                v0 = Mathf.Max(_minJumpForce, v0);

                initialVelocity = new Vector3(v0 * Mathf.Cos(jumpAngle), v0 * Mathf.Sin(jumpAngle), 0);
                
                // Debug.Log("initialVelocity: " + initialVelocity);
            } else {
                // Debug.Log("Heighty jump");
                // Debug.Log("Elevation difference: " + elevationDifference);
                var jumpAngle = _jumpAngle * Mathf.Deg2Rad;
                
                // Tune jump height
                targetPos.y += Mathf.Abs(elevationDifference - 0.5f) * 0.3f + (distance - 2.5f) * 0.9f;
                
                // Formula retrieved by: 
                // 1. Time in the air in the y-direction equation
                // => (target y) = (initial y) + (v0 * sin(jumpAngle) * t) - 1/2gt^2, where v0 = initial velocity
                // 2. Solve for the time in the air in the x-direction -> _dashSpeed is the velocity in the x-direction
                // => t = (distance) / (_dashSpeed)
                // 3. Substitute #2 back into #1 and solve for v0
                var v0 = (targetPos.y - heroPos.y + 0.5f * gravity * (distance / _dashSpeed * distance / _dashSpeed)) /
                         (Mathf.Sin(jumpAngle) * (distance / _dashSpeed));

                initialVelocity = new Vector3(_dashSpeed, Mathf.Max(_minJumpForce, v0) * Mathf.Sin(jumpAngle), 0);
            }
            
            Parent.SetData("JumpAngle", initialVelocity);
            
            // Debug.Log("Target: " + targetPos);
            // Debug.Log("Launch position: " + heroPos);
            // Debug.Log("Jump force: " + v0);
            
            // Note that we will have to compare the maximum jumpForce between the initial velocity needed to clear the
            // tallest trap and the target point to reach

            return NodeStatus.SUCCESS;
        }
    }
}