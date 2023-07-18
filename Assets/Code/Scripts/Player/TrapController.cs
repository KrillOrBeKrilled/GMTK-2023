using System.Collections.Generic;
using System.Linq;
using Traps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Code.Scripts.Player.Input
{
    //*******************************************************************************************
    // TrapController
    //*******************************************************************************************
    /// <summary>
    /// A class to handle the eligibility calculation of tile placement called every frame
    /// through tilemap and custom collision detection. Contains helper methods to check tile
    /// types, validate (paint green), invalidate (paint red), and clear the deployment grid.
    /// </summary>
    public class TrapController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _trapPrefabs;
        public List<Trap> Traps => _trapPrefabs.Select(prefab => prefab.GetComponent<Trap>()).ToList();
        public int CurrentTrapIndex { get; set; }
        
        public Tilemap TileMap { get; }
        public List<Vector3Int> PreviousTilePositions { get; private set; }
        
        [SerializeField] private GameObject _leftDeployTransform, _rightDeployTransform;
        private readonly Transform[] _deployTransforms = new Transform[2];
        private readonly TrapOverlap[] _deployColliders = new TrapOverlap[2];

        public bool IsColliding { get; set; }
        public bool CanDeploy { get; private set; }

        private void Awake()
        {
            PreviousTilePositions = new List<Vector3Int>();
                
            _deployTransforms[0] = _leftDeployTransform.GetComponent<Transform>();
            _deployTransforms[1] = _rightDeployTransform.GetComponent<Transform>();
            
            _deployColliders[0] = _leftDeployTransform.GetComponent<TrapOverlap>();
            _deployColliders[1] = _rightDeployTransform.GetComponent<TrapOverlap>();
        }

        public void SurveyTrapDeployment(bool isGrounded, float direction)
        {
            if (!isGrounded) return;

            // Check whether to deploy left or right
            var deployCheckerIndex = direction < 0 ? 0 : 1;
            var deploymentOrigin = TileMap.WorldToCell(_deployTransforms[deployCheckerIndex].position);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (PreviousTilePositions.Count < 1 || deploymentOrigin != PreviousTilePositions[0])
            {
                // The tile changed, so flush the tint on the previous tiles and reset the collision status
                ClearTrapDeployment();

                // Get the grid placement data for the selected prefab
                var selectedTrapPrefab = Traps[CurrentTrapIndex];
                var prefabPoints = direction < 0
                        ? selectedTrapPrefab.GetLeftGridPoints()
                        : selectedTrapPrefab.GetRightGridPoints();

                // Validate the deployment of the trap with a validation score
                var validationScore = 0;
                var currentCollision = _deployColliders[deployCheckerIndex].GetCollisionData();

                if (currentCollision)
                {
                    // Simulate a broad phase of collision; if there's something in the general area, check if any of
                    // the tiles to be painted are within the collision bounds
                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        var tileSpacePosition = deploymentOrigin + prefabOffsetPosition;

                        // Validation score is based on if the trap prefab overlaps a specific tile of type TrapTile
                        validationScore = IsTileOfType<TrapTile>(TileMap, tileSpacePosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        TileMap.SetTileFlags(tileSpacePosition, TileFlags.None);
                        PreviousTilePositions.Add(tileSpacePosition);

                        // Check tile collision
                        if (!IsColliding) CheckTileCollision(currentCollision, tileSpacePosition);
                    }

                    if (IsColliding)
                    {
                        InvalidateTrapDeployment();
                        return;
                    }
                }
                else
                {
                    IsColliding = false;

                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        validationScore = IsTileOfType<TrapTile>(TileMap, deploymentOrigin + prefabOffsetPosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        TileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                        PreviousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
                    }
                }

                // If the validation score isn't high enough, paint the selected tiles an invalid color
                if (!selectedTrapPrefab.IsValidScore(validationScore))
                {
                    InvalidateTrapDeployment();
                    return;
                }

                ValidateTrapDeployment();
            }

            // Check that a trap is not already placed there
            if (IsColliding) InvalidateTrapDeployment();
        }
        
        //========================================
        // Helper Methods
        //========================================
        
        private static bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase
        {
            var targetTile = tilemap.GetTile(position);
            return targetTile && targetTile is T;
        }
        
        private void CheckTileCollision(Collider2D currentCollision, Vector3Int tileSpacePosition)
        {
            // Convert the origin tile position to world space
            var tileWorldPosition = TileMap.CellToWorld(tileSpacePosition);

            // Check that the tile unit is not within the collision bounds
            var bounds = currentCollision.bounds;
            var maxBounds = bounds.max;
            var minBounds = bounds.min;

            var vertices1 = (tileWorldPosition.x <= maxBounds.x) && (tileWorldPosition.y <= maxBounds.y);
            var vertices2 = (tileWorldPosition.x >= minBounds.x) && (tileWorldPosition.y >= minBounds.y);

            // If any tile is found within the collider, invalidate the deployment
            if (vertices1 && vertices2)
            {
                IsColliding = true;
            }
        }

        private void InvalidateTrapDeployment()
        {
            // Paint all the tiles red
            foreach (var previousTilePosition in PreviousTilePositions)
            {
                TileMap.SetColor(previousTilePosition, new Color(1, 0, 0, 0.3f));
            }

            CanDeploy = false;
        }

        private void ValidateTrapDeployment()
        {
            // Paint all the tiles green
            foreach (var previousTilePosition in PreviousTilePositions)
            {
                TileMap.SetColor(previousTilePosition, new Color(0, 1, 0, 0.3f));
            }

            CanDeploy = true;
        }

        public void ClearTrapDeployment()
        {
            foreach (var previousTilePosition in PreviousTilePositions)
            {
                TileMap.SetColor(previousTilePosition, new Color(1, 1, 1, 0));
            }
            CanDeploy = false;

            // Clear the data of the previous tile
            PreviousTilePositions.Clear();
        }
        
        //========================================
        // Getters & Setters
        //========================================
        public GameObject GetCurrentTrap()
        {
            return _trapPrefabs[CurrentTrapIndex];
        }
    }
}