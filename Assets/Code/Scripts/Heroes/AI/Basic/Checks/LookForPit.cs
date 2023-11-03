using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// LookForPit
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class LookForGround : Node {
        private readonly Transform _heroTransform;
        private readonly FieldOfView _heroSight;
        private readonly LayerMask _groundLayers;
        
        public LookForGround(Transform heroTransform, FieldOfView heroSight, LayerMask groundLayers) {
            _heroTransform = heroTransform;
            _heroSight = heroSight;
            _groundLayers = groundLayers;
        }
        
        internal override NodeStatus Evaluate() {
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");

            if (pitList.Count > 0) {
                var heroPos = _heroTransform.position;
                var nextEntryLaunchPoint = pitList[0].Item1;

                // If the hero passes the next target jump launch point while midair, unregister it
                if (nextEntryLaunchPoint.x < heroPos.x && Mathf.Abs(nextEntryLaunchPoint.x - heroPos.x) > 0.5f) {
                    pitList.RemoveAt(0);
                }
            }

            // Check if a wall is in front of the hero
            var wallHit = Physics2D.Raycast(
                _heroTransform.position + Vector3.down * 1.8f, 
                Vector2.right, 
                8f, 
                _groundLayers
            );

            var lastLedge = (Vector3)GetData("LastSeenLedge");
            var launchLedgePos = Vector3.zero;
            var targetLedgePos = Vector3.zero;

            if (_heroSight.CheckForPit(out var optionCount, out var groundHitData, out var pitEndpoints, _groundLayers)) {
                // Before adding it to the list, make sure this action is not already registered
                if (lastLedge != Vector3.zero && groundHitData.point.x - lastLedge.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }

                launchLedgePos = (Vector3)groundHitData.point;

                // Set the jump land endpoint if there are any that have been sighted. Otherwise, leave it blank
                targetLedgePos = Vector3.zero;
                if (optionCount > 0) {
                    // The jump land point will be selected randomly for now
                    var randomNum = Random.Range(0, optionCount);
                    var pitToJump = pitEndpoints[randomNum];
            
                    targetLedgePos = _heroSight.FindJumpEndpoint(pitToJump);
                }
            } else if (wallHit) {
                var jumpPos = wallHit.point + Vector2.left * 1.3f;
                
                // Before adding it to the list, make sure this action is not already registered
                if (lastLedge != Vector3.zero && jumpPos.x - lastLedge.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }
                
                launchLedgePos = jumpPos;

                var adjustedHitPos = wallHit.point;
                adjustedHitPos.x += 0.5f;
                targetLedgePos = _heroSight.FindJumpEndpoint(adjustedHitPos);
                
                Debug.Log("Jump position: " + wallHit.point);
                Debug.Log("Endpoint: " + targetLedgePos);
            }

            if (targetLedgePos == Vector3.zero) {
                return NodeStatus.SUCCESS;
            }
            
            // Insert in an orderly fashion
            int i;
            for (i = 0; i < pitList.Count; i++) {
                var jumpLaunchPoint = pitList[i].Item1;

                if (launchLedgePos.x > jumpLaunchPoint.x) {
                    continue;
                }
                
                Debug.Log("launch ledge position to be added: " + launchLedgePos);
                Debug.Log("jump launch position to be compared: " + jumpLaunchPoint);

                break;
            }
            
            pitList.Insert(i, (launchLedgePos, targetLedgePos));
            Parent.SetData("LastSeenLedge", launchLedgePos);

            // Debug.Log("Enqueued!");
            // Debug.Log("Launch position: " + launchLedgePos);
            // Debug.Log("Land position: " + targetLedgePos);

            return NodeStatus.SUCCESS;
        }
    }
}