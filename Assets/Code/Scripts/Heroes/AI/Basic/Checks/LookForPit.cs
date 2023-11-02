using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// LookForPit
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class LookForPit : Node {
        private readonly Transform _heroTransform;
        private readonly FieldOfView _heroSight;
        private readonly LayerMask _groundLayers;
        
        public LookForPit(Transform heroTransform, FieldOfView heroSight, LayerMask groundLayers) {
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

            if (!_heroSight.CheckForPit(out var optionCount, out var groundHitData, out var pitEndpoints, _groundLayers)) {
                return NodeStatus.SUCCESS;
            }

            var lastLedge = (Vector3)GetData("LastSeenLedge");
            
            // Before adding it to the list, make sure this action is not already registered
            if (groundHitData.point.x - lastLedge.x < 1.1f) {
                return NodeStatus.SUCCESS;
            }

            // Make a slight adjustment to the jump launch position
            var launchLedgePos = (Vector3)groundHitData.point;
            // launchLedgePos.x += 0.5f;

            // Set the jump land endpoint if there are any that have been sighted. Otherwise, leave it blank
            var targetLedgePos = Vector3.zero;
            if (optionCount > 0) {
                // The jump land point will be selected randomly for now
                var randomNum = Random.Range(0, optionCount);
                var pitToJump = pitEndpoints[randomNum];
            
                targetLedgePos = _heroSight.FindJumpEndpoint(pitToJump);
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
            // Debug.Log(pitList.Count);
            Parent.SetData("LastSeenLedge", launchLedgePos);

            // Debug.Log("Enqueued!");
            // Debug.Log("Launch position: " + launchLedgePos);
            // Debug.Log("Land position: " + targetLedgePos);

            return NodeStatus.SUCCESS;
        }
    }
}