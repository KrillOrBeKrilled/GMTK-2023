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
            var pitQueue = (Queue<(Vector3, Vector3)>)GetData("PitQueue");

            if (pitQueue.Count > 0) {
                var heroPos = _heroTransform.position;
                var nextEntryLaunchPoint = pitQueue.Peek().Item1;

                // If the hero passes the next target jump launch point while midair, unregister it
                if (nextEntryLaunchPoint.x < heroPos.x && Mathf.Abs(nextEntryLaunchPoint.x - heroPos.x) > 0.5f) {
                    pitQueue.Dequeue();
                }
            }

            if (!_heroSight.CheckForPit(out var optionCount, out var groundHitData, out var pitEndpoints, _groundLayers)) {
                return NodeStatus.SUCCESS;
            }

            var lastLedge = (Vector3)GetData("LastSeenLedge");
            
            // Before enqueueing, make sure this action is not already registered
            if (groundHitData.point.x - lastLedge.x < 1.1f) {
                return NodeStatus.SUCCESS;
            }

            // Make a slight adjustment to the jump launch position
            var launchLedgePos = (Vector3)groundHitData.point;
            launchLedgePos.x += 0.5f;

            // Set the jump land endpoint if there are any that have been sighted. Otherwise, leave it blank
            var targetLedgePos = Vector3.zero;
            if (optionCount > 0) {
                // The jump land point will be selected randomly for now
                var randomNum = Random.Range(0, optionCount);
                var pitToJump = pitEndpoints[randomNum];
            
                targetLedgePos = _heroSight.FindJumpEndpoint(pitToJump);
                targetLedgePos.y += 1.5f;
            }

            pitQueue.Enqueue((launchLedgePos, targetLedgePos));
            Parent.SetData("LastSeenLedge", launchLedgePos);
            
            Debug.Log("Enqueued!");
            Debug.Log("Launch position: " + launchLedgePos);
            Debug.Log("Land position: " + targetLedgePos);

            return NodeStatus.SUCCESS;
        }
    }
}