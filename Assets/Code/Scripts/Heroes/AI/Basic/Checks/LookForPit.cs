using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// LookForPit
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    public class LookForPit : Node {
        private readonly FieldOfView _heroSight;
        private readonly Tilemap _groundTilemap;
        private readonly LayerMask _groundLayers;
        private readonly float _elevationDisparityThreshold;
        
        public LookForPit(FieldOfView heroSight, Tilemap groundTilemap, LayerMask groundLayers, float elevationDisparityThreshold) {
            _heroSight = heroSight;
            _groundTilemap = groundTilemap;
            _groundLayers = groundLayers;
            _elevationDisparityThreshold = elevationDisparityThreshold;
        }
        
        internal override NodeStatus Evaluate() {
            if (_heroSight.CheckForGround(out var groundHitData, _groundLayers)) {
                var prevGroundPos = (Vector3)GetData("LastSeenGroundPos");
                var currGroundPos = (Vector3)groundHitData.point;
                
                // Debug.Log(prevGroundPos);
                // Debug.Log(currGroundPos);
                // Debug.Log(prevGroundPos.y - currGroundPos.y);

                if ((bool)GetData("IsTrackingPit")) {
                    // The pit has ended, so add the complete pit data to the PitQueue
                    var pitQueue = (Queue<(Vector3, Vector3)>)GetData("PitQueue");
                    var ledgePos = (Vector3)GetData("LastSeenGroundPos");

                    var tilePos = _groundTilemap.WorldToCell(groundHitData.point);
                    while (_groundTilemap.GetTile(tilePos)) {
                        tilePos.y += 1;
                    }

                    var nextLedgePos = _groundTilemap.GetCellCenterWorld(tilePos);
                    nextLedgePos.y += 1.5f;
                    // Debug.Log("Target jump position: " + nextLedgePos);
                    Debug.Log("Push to Queue");
                    
                    pitQueue.Enqueue((ledgePos, nextLedgePos));
                    
                    // Wipe the temporary pit-tracking data
                    Parent.SetData("LastSeenGroundPos", Vector3.zero);
                    Parent.SetData("IsTrackingPit", false);

                    return NodeStatus.SUCCESS;
                }
                
                if (prevGroundPos != Vector3.zero && Mathf.Abs(prevGroundPos.y - currGroundPos.y) > _elevationDisparityThreshold) {
                    // In the case that the FoV is large enough to see all parts of the pit ground at any given time,
                    // start tracking for a pit when there's a great disparity in the elevation
                    Parent.SetData("IsTrackingPit", true);
                    
                    Debug.Log("Begin tracking pit");

                    return NodeStatus.SUCCESS;
                }
                
                Parent.SetData("LastSeenGroundPos", (Vector3)groundHitData.point);
            } else {
                if ((bool)GetData("AbortPitTracking")) {
                    // If the hero gets dangerously close to a pit that hasn't been fully mapped out, emergency push
                    // to the PitQueue with unfinished data
                    var pitQueue = (Queue<(Vector3, Vector3)>)GetData("PitQueue");
                    var ledgePos = (Vector3)GetData("LastSeenGroundPos");
                    
                    Debug.Log("Abort tracking enqueue");
                    pitQueue.Enqueue((ledgePos, Vector3.zero));
                    
                    Parent.SetData("LastSeenGroundPos", Vector3.zero);
                    Parent.SetData("AbortPitTracking", false);
                    Parent.SetData("IsTrackingPit", false);

                    return NodeStatus.SUCCESS;
                }

                // If the ground is not seen this frame, notify the system that a pit is coming up
                Parent.SetData("IsTrackingPit", true);
            }
            
            return NodeStatus.SUCCESS;
        }
    }
}