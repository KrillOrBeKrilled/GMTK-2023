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
        private readonly LayerMask _trapLayers;

        private Vector3 _lastSeenLedge = Vector3.zero;
        private Vector3 _lastSeenWall = Vector3.zero;
        
        public LookForGround(Transform heroTransform, FieldOfView heroSight, LayerMask groundLayers, LayerMask trapLayers) {
            _heroTransform = heroTransform;
            _heroSight = heroSight;
            _groundLayers = groundLayers;
            _trapLayers = trapLayers;
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
                _heroTransform.position + Vector3.down * 1.6f, 
                Vector2.right, 
                8f, 
                _groundLayers
            );
            
            var launchLedgePos = Vector3.zero;
            var targetLedgePos = Vector3.zero;
            var sawPit = false;

            if (_heroSight.CheckForPit(out var optionCount, out var groundHitData, out var pitEndpoints, _groundLayers, _trapLayers)) {
                sawPit = true;
                
                // Before adding it to the list, make sure this action is not already registered
                if (_lastSeenLedge != Vector3.zero && groundHitData.point.x - _lastSeenLedge.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }

                launchLedgePos = (Vector3)groundHitData.point;
                // launchLedgePos.x -= 0.2f;

                // Set the jump land endpoint if there are any that have been sighted. Otherwise, leave it blank
                targetLedgePos = Vector3.zero;
                if (optionCount > 0) {
                    // The jump land point will be selected randomly for now
                    var randomNum = Random.Range(0, optionCount);
                    var pitToJump = pitEndpoints[randomNum];
            
                    targetLedgePos = _heroSight.FindJumpEndpoint(pitToJump);
                    
                    // Debug.Log("Pit jump position: " + launchLedgePos);
                    // Debug.Log("Pit jump endpoint: " + targetLedgePos);
                }
            } else if (wallHit && !(bool)GetData("IsFalling")) {
                var jumpPos = wallHit.point + Vector2.left * 3.5f;
                
                // Before adding it to the list, make sure this action is not already registered
                if (_lastSeenWall != Vector3.zero && jumpPos.x - _lastSeenWall.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }
                
                launchLedgePos = jumpPos;
                // launchLedgePos.x -= 0.2f;

                var adjustedHitPos = wallHit.point;
                adjustedHitPos.x += 0.5f;
                targetLedgePos = _heroSight.FindJumpEndpoint(adjustedHitPos);
                
                // Debug.Log("Wall jump position: " + launchLedgePos);
                // Debug.Log("Wall jump endpoint: " + targetLedgePos);
            }

            if (targetLedgePos == Vector3.zero) {
                return NodeStatus.SUCCESS;
            }
            
            // Insert in an orderly fashion
            // Debug.Log("Inserting into list!");
            int i;
            for (i = 0; i < pitList.Count; i++) {
                var jumpLaunchPoint = pitList[i].Item1;
                
                // Debug.Log("Jump point: " + jumpLaunchPoint);
                // Debug.Log("End point: " + pitList[i].Item2);

                if (launchLedgePos.x > jumpLaunchPoint.x) {
                    continue;
                }
                
                // Debug.Log("launch ledge position to be added: " + launchLedgePos);
                // Debug.Log("jump launch position to be compared: " + jumpLaunchPoint);

                break;
            }
            
            // Debug.Log("jump position being added: " + launchLedgePos);
            // Debug.Log("jump end position being added: " + targetLedgePos);
            
            pitList.Insert(i, (launchLedgePos, targetLedgePos));
            
            if (sawPit) {
                _lastSeenLedge = launchLedgePos;
            } else {
                _lastSeenWall = launchLedgePos;
            }

            // Debug.Log("Enqueued!");
            // Debug.Log("Launch position: " + launchLedgePos);
            // Debug.Log("Land position: " + targetLedgePos);

            return NodeStatus.SUCCESS;
        }
    }
}