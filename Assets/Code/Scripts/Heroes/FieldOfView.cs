using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KrillOrBeKrilled.Heroes {
    public class FieldOfView : MonoBehaviour {
        [Header("Debug")]
        [Tooltip("Draws the field of view area in the scene view and highlights sighted objects.")]
        public bool Debug;
        [Tooltip("Highlights the field of view area when there are sighted objects.")]
        public bool ObjectCheck;
        [Tooltip("Highlights the field of view area when the ground has been sighted.")]
        public bool PitCheck;
        [Tooltip("The layer of GameObjects to sight in the debug view.")]
        public LayerMask TargetMask;
        
        [Header("Field of View Configuration")]
        [Tooltip("Offsets the position of this GameObject to act as the origin of the field of view area.")]
        public Vector3 Offset;

        [Tooltip("The farsightedness of this GameObject.")]
        public float InnerRadius = 0;
        [Tooltip("The maximum distance this GameObject can 'see'.")]
        public float OuterRadius = 6;

        [Tooltip("The angle of the field of view wedge area.")]
        [Range(45, 180)]
        public float FovDegree = 45;
        
        [Tooltip("The maximum number of targets this GameObject can denote within the field of view.")]
        public int TargetThreshold = 10;

        [Tooltip("The minimum distance required between this GameObject and the target land point to consider as a " +
                 "viable option to jump past a pit.")]
        public float pitOptionDistanceThreshold;
        
        public float LineStuff = 6;
        
        private float FovRadians => FovDegree * Mathf.Deg2Rad;
        private float AngThreshold => Mathf.Cos(FovRadians / 2);
        private Tilemap _groundTilemap;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void OnDrawGizmos() {
            if (!Debug) {
                return;
            }
            
            Gizmos.color = Color.green;
            // Draw the raycast to check when the hero touches the ground
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * LineStuff));

            // Perform calculations in local space
            Gizmos.matrix = Handles.matrix = transform.localToWorldMatrix;
            
            // Draw OverlapCircle boundaries
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Offset, OuterRadius);

            var targets = new List<Collider2D>();
            var hit = new RaycastHit2D();
            var pitEndpoints = new List<Vector2>();

            var foundTarget = ObjectCheck && FOVContains(out targets, TargetMask);
            var foundGround = PitCheck && CheckForPit(out var pitOptions, out hit, out pitEndpoints, TargetMask);
            
            if (foundGround || foundTarget) {
                Gizmos.color = Handles.color = Color.red;
            } else {
                Gizmos.color = Handles.color = Color.magenta;
            }

            // Draw origin of the FOV wedge
            Gizmos.DrawSphere(Offset, 0.1f);

            var p = AngThreshold;
            var x = Mathf.Sqrt(1 - p * p);
            
            var vTopDir = new Vector3(p, x, 0);
            var vBottomDir = new Vector3(p, -x, 0);
            var vTopOuter = vTopDir * OuterRadius;
            var vBottomOuter = vBottomDir * OuterRadius;
            var vTopInner = vTopDir * InnerRadius;
            var vBottomInner = vBottomDir * InnerRadius;

            // Draw arcs along the outer and inner radii
            Handles.DrawWireArc(Offset, Vector3.forward, vBottomOuter, FovDegree, OuterRadius);
            Handles.DrawWireArc(Offset, Vector3.forward, vBottomInner, FovDegree, InnerRadius);

            // Draw lines connecting the arcs
            Gizmos.DrawLine(vTopInner + Offset, vTopOuter + Offset);
            Gizmos.DrawLine(vBottomInner + Offset, vBottomOuter + Offset);
            
            // Draw points for each target that lies inside the vision wedge
            foreach (var sightedTarget in targets) {
                Gizmos.DrawSphere(sightedTarget.bounds.center - transform.position, .2f);
            }

            if (!foundGround) {
                return;
            }
            
            // Draw the raycast hit for each pit endpoint option
            foreach (var endpoint in pitEndpoints) {
                Gizmos.DrawSphere((Vector3)endpoint - transform.position, .2f);
            }
                
            // Draw the first raycast hit with the ground
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3)hit.point - transform.position, .2f);
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        internal bool FOVContains(out List<Collider2D> targets, LayerMask layer) {
            var eyeOrigin = Offset + transform.position;
            targets = new List<Collider2D>();
            
            var broadPhaseTargets = new Collider2D[TargetThreshold];
            var numTargets = 
                Physics2D.OverlapCircleNonAlloc(eyeOrigin, OuterRadius, broadPhaseTargets, layer);

            for (var i = 0; i < numTargets; i++) {
                var broadPhaseTarget = broadPhaseTargets[i];
                var sightedTarget = broadPhaseTarget.transform.position;
                
                // Optimize by immediately ruling out any targets behind the sight wedge origin
                if (sightedTarget.x < transform.position.x) {
                    continue;
                }
                
                // Test that no other collider is obscuring the targeted collider
                var hit = Physics2D.Raycast(eyeOrigin, (broadPhaseTarget.bounds.center - eyeOrigin).normalized, OuterRadius * 2f, layer);
                if (hit && hit.collider.gameObject != broadPhaseTarget.gameObject) {
                    continue;
                }

                var bounds = broadPhaseTarget.bounds;
                var targetPos = bounds.center;
                var colliderExtents = bounds.extents;

                // We can make this even "smarter" by not just checking the corners of the colliders, but for now
                // let's take the cheaper route of four corners
                var colliderEdges = new Vector3[] {
                    targetPos + colliderExtents,
                    new(targetPos.x + colliderExtents.x, targetPos.y - colliderExtents.y, 0),
                    new(targetPos.x - colliderExtents.x, targetPos.y + colliderExtents.y, 0),
                    new(targetPos.x - colliderExtents.x, targetPos.y - colliderExtents.y, 0)
                };

                for (var j = 0; j < 4; j++) {
                    // Transform direction to target in local space to make the calculations simpler
                    var vecToTargetWorld = colliderEdges[j] - eyeOrigin;
                    var vecToTarget = transform.InverseTransformDirection(vecToTargetWorld);

                    // Radial check
                    if (vecToTarget.magnitude < InnerRadius || vecToTarget.magnitude > OuterRadius) {
                        continue;
                    }
                    
                    // Angular check
                    if (vecToTarget.normalized.x < AngThreshold) {
                        continue;
                    }

                    targets.Add(broadPhaseTarget);
                    break;
                }
            }
            
            // Once the targets have been sighted, order them by approximate distance from this GameObject
            targets = targets.OrderBy(target => (eyeOrigin - target.bounds.center).sqrMagnitude).ToList();

            return (targets.Count > 0);
        }
        
        internal bool CheckForPit(out int pitOptions, out RaycastHit2D hitData, out List<Vector2> pitEndpoints, LayerMask layer) {
            pitEndpoints = new List<Vector2>();
            var eyeOrigin = Offset + transform.position;
            
            var p = AngThreshold;
            var x = Mathf.Sqrt(1 - p * p);
            
            var vGroundCheckDir = new Vector3(p, -x, 0);

            // Raycast as far as the FoV permits
            hitData = Physics2D.Raycast(eyeOrigin, vGroundCheckDir, OuterRadius, layer);

            if (!hitData) {
                pitOptions = 0;
                return false;
            }
            
            // Sometimes the point is on the edge and tracks the wrong tile, so slightly adjust the position
            var sightedGroundPos = hitData.point;
            var hitTilePos = sightedGroundPos;
            hitTilePos.y -= 0.5f;
            var tilePos = _groundTilemap.WorldToCell(hitTilePos);
            tilePos.x += 1;

            if (_groundTilemap.GetTile(tilePos)) {
                pitOptions = 0;
                return false;
            }
            
            var vBottomDir = new Vector3(p, -x, 0);
            var vBottomOuter = vBottomDir * OuterRadius;
            
            pitOptions = 0;
            var tileLedgeHeight = tilePos.y;
            var tileHeightDistance = Mathf.Abs((int)(vBottomOuter.y - eyeOrigin.y));
            var tilesToGround = Mathf.Abs((int)(eyeOrigin.y - sightedGroundPos.y));
            var remainingSightExtent = eyeOrigin.x + OuterRadius - sightedGroundPos.x;
            var lastPitEndpoint = Vector3.zero;
            
            var currTilePos = tilePos;
            currTilePos.y += tileHeightDistance + tilesToGround;
            
            // Search for the closest platform above the ground the GameObject is standing on
            for (var i = 0; i < tileHeightDistance + tilesToGround; i++) {
                if (_groundTilemap.GetTile(currTilePos)) {
                    // If this tile is not null raycasting will hit the other end of the collider, so skip this entry
                    continue;
                }

                var origin = _groundTilemap.GetCellCenterWorld(currTilePos);
                currTilePos.y--;
                
                // Since there's no tile to the right, raycast as far as the FoV allows to find the end of the pit
                var pitEndpointCheck = Physics2D.Raycast(origin, Vector2.right, remainingSightExtent, layer);

                if (!pitEndpointCheck) {
                    // Couldn't find the end of the pit, so disregard this unit and continue checking downward
                    continue;
                }
                
                // Only register the end of the pit if it's the first endpoint or if the endpoint is closer than the last
                if (lastPitEndpoint != Vector3.zero && (!(pitEndpointCheck.point.x < lastPitEndpoint.x) || 
                    (lastPitEndpoint.x - pitEndpointCheck.point.x) < 0.5f)) {
                    continue;
                }

                lastPitEndpoint = pitEndpointCheck.point;
            }

            if (lastPitEndpoint != Vector3.zero) {
                pitEndpoints.Add(lastPitEndpoint);
                pitOptions++;
            }
            

            currTilePos = tilePos;

            for (var i = 0; i < tileHeightDistance - tilesToGround; i++) {
                if (_groundTilemap.GetTile(currTilePos) && currTilePos.y < tileLedgeHeight) {
                    // Before adding the bottom of the pit, make sure that it extends long enough to walk through safely
                    var groundPos = _groundTilemap.GetCellCenterWorld(currTilePos);
                    
                    pitEndpoints.Add(groundPos);
                    pitOptions++;

                    // The bottom has been reached, so the pit no longer needs to be traced
                    break;
                }

                var origin = _groundTilemap.GetCellCenterWorld(currTilePos);
                currTilePos.y--;
                
                // Since there's no tile to the right, raycast as far as the FoV allows to find the end of the pit
                var pitEndpointCheck = Physics2D.Raycast(origin, Vector2.right, remainingSightExtent, layer);

                if (!pitEndpointCheck) {
                    // Couldn't find the end of the pit, so disregard this unit and continue checking downward
                    continue;
                }
                
                // Only register the end of the pit if it's the first endpoint or if the endpoint is closer than the last
                if (lastPitEndpoint != Vector3.zero && (!(pitEndpointCheck.point.x < lastPitEndpoint.x) || 
                    (lastPitEndpoint.x - pitEndpointCheck.point.x) < 0.5f)) {
                    continue;
                }

                lastPitEndpoint = pitEndpointCheck.point;
                pitEndpoints.Add(lastPitEndpoint);
                pitOptions++;
            }

            return true;
        }

        internal Vector3 FindJumpEndpoint(Vector3 position) {
            var tilePos = _groundTilemap.WorldToCell(position);
            while (_groundTilemap.GetTile(tilePos)) {
                tilePos.y += 1;
            }

            return _groundTilemap.GetCellCenterWorld(tilePos);
        }
        
        internal void Initialize(Tilemap groundTilemap) {
            _groundTilemap = groundTilemap;
        }
        
        #endregion
    }
}
