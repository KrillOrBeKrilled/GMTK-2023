using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// LookForObstacle
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A checker node used to operate the hero AI that governs the hero's jump decision
    /// making by sighting traps and calculating the safest path with regard to the
    /// available pit and wall sightings.
    /// </summary>
    public class LookForObstacle : Node {
        /// <summary>
        /// Checks for jump actions currently available to the hero and updates the jump launch position and endpoint
        /// accordingly.
        /// </summary>
        /// <returns>
        /// The <b>success</b> status if a jump action can or already has been registered.
        /// The <b>failure</b> status if no jump action is available to the hero.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var jumpStartPos = (Vector3)GetData("JumpLaunchPoint");
            var pitList = (List<(Vector3, Vector3)>)GetData("PitList");
            var pitsFound = pitList.Count > 0;
            
            if (jumpStartPos != Vector3.zero) {
                // Check if a new entry in the pitList is closer than the current jump target and update the launch point
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
                
            return NodeStatus.SUCCESS;
            
            // TODO: Check for traps logic to be continued below...
        }
    }
}

