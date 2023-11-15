using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// DecideJumpForce
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A checker node used to operate the hero AI that governs the hero's calculation
    /// of the force required to successfully make the current jump in question. The
    /// hero will choose between jump types to make based on the jump landing position.
    /// </summary>
    public class DecideJumpForce : Node {
        private Transform _heroTransform;
        private readonly Rigidbody2D _rigidbody;
        private readonly float _maxJumpForce, _minJumpForce; 
        
        private float _gravity => Physics.gravity.magnitude * _rigidbody.gravityScale;
        private const float _radJumpAngle = 89f * Mathf.Deg2Rad;
        private const float _dashSpeed = 8f;
        
        public DecideJumpForce(Transform heroTransform, Rigidbody2D rigidbody, float minJumpForce, float maxJumpForce) {
            _heroTransform = heroTransform;
            _rigidbody = rigidbody;
            _maxJumpForce = maxJumpForce;
            _minJumpForce = minJumpForce;
        }
        
        /// <summary>
        /// Gauges the distance and elevation difference between the jump launch position and the jump target position
        /// and chooses a jump type calculation to commence to reach the respective target successfully.
        /// </summary>
        /// <remarks> The first formula to be used for short distance jumps to reach higher elevations and longer
        /// distance jumps calculates the initial velocity needed to reach a target apex. The second formula forms
        /// a heighty and fluttery jump by calculating the initial velocity in the vertical direction required to
        /// reach the jump land point. </remarks>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            var targetPos = (Vector3)GetData("JumpLandPoint");
            var launchPos = (Vector3)GetData("JumpLaunchPoint");

            // If no jump endpoint has been defined, the hero will jump with maximum force
            if (targetPos == Vector3.zero) {
                Debug.LogError("Hero cannot find a target platform!");
                
                Parent.SetData("JumpVelocity", 
                    new Vector3(_maxJumpForce / 2, _maxJumpForce * Mathf.Sin(_radJumpAngle), 0));
                
                return NodeStatus.SUCCESS;
            }

            var heroPos = _heroTransform.position;
            var distance = targetPos.x - launchPos.x;

            Vector3 initialVelocity;
            var elevationDifference = targetPos.y - launchPos.y;

            if (elevationDifference > 1f || distance > 5.5f) {
                var positiveElevationDifference = Mathf.Abs(elevationDifference);
                
                // Magic calculation to offset the target apex position to successfully land atop the platform
                var jumpApex = targetPos.y < launchPos.y
                    ? new Vector3(
                        distance * 1 / positiveElevationDifference * 0.28f + (distance - 6.35f) * 0.28f + 0.92f,
                        1 / positiveElevationDifference * 0.03f + distance * 0.44f + 1f,
                        0
                    )
                    : new Vector3(
                        distance * 0.35f,
                        positiveElevationDifference + positiveElevationDifference * 0.03f + distance * 0.45f + 1f,
                        0
                    );

                var jumpAngle = Mathf.Asin(jumpApex.normalized.y);

                // Use Toricelli's formula to get initial velocity to reach the apex
                var v0 = Mathf.Sqrt(2 * _gravity * Mathf.Max(0, jumpApex.y));
                v0 = Mathf.Clamp(v0, _minJumpForce, _maxJumpForce);

                initialVelocity = new Vector3(v0 * Mathf.Cos(jumpAngle), v0 * Mathf.Sin(jumpAngle), 0);
            } else {
                // Magic calculation to offset the target landing position to successfully land atop the platform
                targetPos.y += Mathf.Abs(elevationDifference - 0.5f) * 0.3f + (distance - 2.5f) * 0.7f;
                
                // Formula retrieved by: 
                // 1. Time in the air in the y-direction equation
                // => (target y) = (initial y) + (v0 * sin(jumpAngle) * t) - 1/2gt^2, where v0 = initial velocity
                // 2. Solve for the time in the air in the x-direction -> _dashSpeed is the velocity in the x-direction
                // => t = (distance) / (_dashSpeed)
                // 3. Substitute #2 back into #1 and solve for v0
                var vx = distance / _dashSpeed;
                var v0 = (targetPos.y - heroPos.y + 0.5f * _gravity * (vx * vx)) /
                         (Mathf.Sin(_radJumpAngle) * vx);
                v0 = Mathf.Clamp(v0, _minJumpForce, _maxJumpForce);
            
                initialVelocity = new Vector3(_dashSpeed, v0 * Mathf.Sin(_radJumpAngle), 0);
            }
            
            Parent.SetData("JumpVelocity", initialVelocity);

            return NodeStatus.SUCCESS;
        }
    }
}