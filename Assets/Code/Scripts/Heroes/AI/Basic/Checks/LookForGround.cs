using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// LookForGround
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A checker node used to operate the hero AI that governs the hero's sighting of
    /// pits and walls and ability to choose the jump path out of the options available.
    /// </summary>
    public class LookForGround : Node {
        private readonly Transform _heroTransform;
        private readonly FieldOfView _heroSight;
        private readonly LayerMask _groundLayers;
        private readonly LayerMask _trapLayers;

        /// Tracks the last pit ledge entered into the jump action list to prevent repeated entries.
        private Vector3 _lastSeenLedge = Vector3.zero;
        /// Tracks the last wall jump entered into the jump action list to prevent repeated entries.
        private Vector3 _lastSeenWall = Vector3.zero;
        
        /// <summary>
        /// Initializes all requisite data for the successful operation of this <see cref="Node"/>;.
        /// </summary>
        /// <param name="heroTransform"> Provides the hero position to check when to purge outdated jump actions. </param>
        /// <param name="heroSight"> Acts as the hero's eyesight, providing pit and ledge-sighting logic. </param>
        /// <param name="groundLayers"> The LayerMask used to check for the ground. </param>
        /// <param name="trapLayers"> The LayerMask used to check for traps in pits. </param>
        public LookForGround(Transform heroTransform, FieldOfView heroSight, LayerMask groundLayers, LayerMask trapLayers) {
            this._heroTransform = heroTransform;
            this._heroSight = heroSight;
            this._groundLayers = groundLayers;
            this._trapLayers = trapLayers;
        }
        
        /// <summary>
        /// Checks for a pit in the ground and registers a jump action with a jump endpoint of choice from the
        /// sighting data. If a pit is not found, checks for a wall directly ahead of the hero and registers
        /// its start and endpoint as a jump action when found.
        /// </summary>
        /// <remarks> The pit endpoint of choice is selected randomly. When a new jump action is registered, the
        /// action is recorded in the managed bookkeeping structure in order of horizontal distance from the hero,
        /// where the closest jump launch position is at the front. </remarks>
        /// <returns> The <b>success</b> status. </returns>
        internal override NodeStatus Evaluate() {
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");

            // Auto clean the pitList if the hero passes any jump waypoints
            if (pitList.Count > 0) {
                var heroPos = this._heroTransform.position;
                var nextEntryLaunchPoint = pitList[0].Item1;

                // If the hero passes the next target jump launch point while midair, unregister it
                if (nextEntryLaunchPoint.x < heroPos.x && Mathf.Abs(nextEntryLaunchPoint.x - heroPos.x) > 2f) {
                    pitList.RemoveAt(0);
                }
            }

            // Check if a wall is in front of the hero
            var wallHit = Physics2D.Raycast(
                this._heroTransform.position + Vector3.down * 1.6f, 
                Vector2.right, 
                4f, 
                this._groundLayers
            );
            
            var launchLedgePos = Vector3.zero;
            var targetLedgePos = Vector3.zero;

            if (this._heroSight.CheckForPit(out var optionCount, out var groundHitData, 
                    out var pitEndpoints, this._groundLayers, this._trapLayers, false)) {
                // Before adding it to the list, make sure this action is not already registered
                if (this._lastSeenLedge != Vector3.zero && groundHitData.point.x - this._lastSeenLedge.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }

                launchLedgePos = groundHitData.point;
                launchLedgePos.x -= 0.3f;

                // Set the jump land endpoint if there are any that have been sighted. Otherwise, leave it blank
                targetLedgePos = Vector3.zero;
                if (optionCount > 0) {
                    // The jump land point will be selected randomly for now
                    var randomNum = Random.Range(0, optionCount);
                    var pitToJump = pitEndpoints[randomNum];
            
                    targetLedgePos = this._heroSight.FindJumpEndpoint(pitToJump);
                }
                
                this._lastSeenLedge = launchLedgePos;
            } else if (wallHit && !(bool)GetData("IsFalling")) {
                var jumpPos = wallHit.point + Vector2.left * 2f;
                
                // Before adding it to the list, make sure this action is not already registered
                if (this._lastSeenWall != Vector3.zero && jumpPos.x - this._lastSeenWall.x < 1.1f) {
                    return NodeStatus.SUCCESS;
                }
                
                launchLedgePos = jumpPos;

                var adjustedHitPos = wallHit.point;
                adjustedHitPos.x += 0.5f;
                targetLedgePos = this._heroSight.FindJumpEndpoint(adjustedHitPos);
                
                this._lastSeenWall = launchLedgePos;
            }

            if (targetLedgePos == Vector3.zero) {
                return NodeStatus.SUCCESS;
            }
            
            int i;
            for (i = 0; i < pitList.Count; i++) {
                var jumpLaunchPoint = pitList[i].Item1;

                if (launchLedgePos.x > jumpLaunchPoint.x) {
                    continue;
                }

                break;
            }
            
            pitList.Insert(i, (launchLedgePos, targetLedgePos));

            return NodeStatus.SUCCESS;
        }
    }
}