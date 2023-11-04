using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// LookForObstacle
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class LookForObstacle : Node {
        private FieldOfView _heroSight;
        private LayerMask _obstacleLayers;
        
        public LookForObstacle(FieldOfView heroSight, LayerMask obstacleLayers) {
            _heroSight = heroSight;
            _obstacleLayers = obstacleLayers;
        }
        
        internal override NodeStatus Evaluate() {
            var jumpStartPos = (Vector3)GetData("JumpLaunchPoint");
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");
            var pitsFound = pitList.Count > 0;
            
            if (jumpStartPos != Vector3.zero) {
                // Check if a new entry in the pitList is closer than the current jump target
                if (!pitsFound || !(pitList[0].Item1.x < jumpStartPos.x)) {
                    return NodeStatus.SUCCESS;
                }
                
                Parent.SetData("JumpLaunchPoint", pitList[0].Item1);
                Parent.Parent.SetData("JumpLandPoint", pitList[0].Item2);

                return NodeStatus.SUCCESS;
            }
            
            if (!pitsFound) {
                return NodeStatus.FAILURE;
            }
            
            var pitData = pitList[0];
            Parent.SetData("JumpLaunchPoint", pitData.Item1);
            Parent.Parent.SetData("JumpLandPoint", pitData.Item2);
                
            // Debug.Log("Set launch position to track: " + pitData.Item1);
            // Debug.Log("Set land position to track: " + pitData.Item2);
                
            return NodeStatus.SUCCESS;

            // // Check for updates that can be made to the jump start point and end point every frame
            // if (_heroSight.FOVContains(out var obstacles, _obstacleLayers)) {
            //     var maxObstacleHeight = -1f;
            //
            //     // Compare the closest obstacle to the jump start point if it exists, otherwise define the nearest
            //     // obstacle as the jump start point
            //     if (jumpStartPos != Vector3.zero) {
            //         // If the jumpStartPos is changed, erase the jumpEndPos for full processing of the obstacle list
            //         jumpStartPos = obstacles[0].bounds.center.x - obstacles[0].bounds.extents.x < jumpStartPos.x
            //             ? obstacles[0].bounds.center - obstacles[0].bounds.extents
            //             : jumpStartPos;
            //         
            //         // Or check if a detected ledge position is closer 
            //         // if (ledgePos != Vector3.zero) {
            //         //     jumpStartPos = ledgePos.x < jumpStartPos.x ? ledgePos : jumpStartPos;
            //         // }
            //         
            //     } else {
            //         jumpStartPos = obstacles[0].bounds.center - obstacles[0].bounds.extents;
            //     }
            //     
            //     Parent.SetData("JumpStartPos", jumpStartPos);
            //     
            //     // Keep in mind that even if a start and end point are already determined, this can still change with new traps
            //     foreach (var obstacle in obstacles) {
            //         // NOTE: This is all in world space
            //         var bounds = obstacle.bounds;
            //         var obstacleCenter = bounds.center;
            //         var obstacleExtents = bounds.extents;
            //
            //         // Only process the newly seen obstacles if a jump was already determined as necessary
            //         if (jumpEndPos != Vector3.zero && (obstacleCenter.x + obstacleExtents.x) < jumpEndPos.x) {
            //             continue;
            //         }
            //         
            //         // TODO: For now we ignore ceiling traps, but will have to deal with them later
            //         if (obstacleCenter.y > _heroSight.transform.position.y) {
            //             continue;
            //         }
            //         
            //         // Find the tallest obstacle extent that the hero must avoid
            //         maxObstacleHeight = Mathf.Max(maxObstacleHeight, obstacleCenter.y + obstacleExtents.y);
            //         
            //         // Update the jumpEndPos to the farthest extent of each trap every loop and then in the next loop
            //         // check if the gap between the new trap and the end of the last is large enough
            //         
            //         // But what if you can't see far enough? The end extent of the last trap saved will be the landing
            //         // point?
            //         // Maybe save whether this happened or not as a bool for the DecideJumpForce to take into account
            //     }
            // }
        }
    }
}

