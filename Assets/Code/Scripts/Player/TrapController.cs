using Audio;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using Traps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
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
        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;

        // ------------- Trap Deployment -------------
        [SerializeField] private List<GameObject> _trapPrefabs;
        public List<Trap> Traps => this._trapPrefabs.Select(prefab => prefab.GetComponent<Trap>()).ToList();
        public int CurrentTrapIndex { get; set; }

        public Tilemap TileMap;
        public List<Vector3Int> PreviousTilePositions { get; private set; }

        [SerializeField] private GameObject _leftDeployTransform, _rightDeployTransform;
        private readonly Transform[] _deployTransforms = new Transform[2];
        private readonly TrapOverlap[] _deployColliders = new TrapOverlap[2];

        public bool IsColliding { get; set; }
        public bool IsSelectingTileSFX { get; set; }
        public bool CanDeploy { get; private set; }

        private void Awake()
        {
            this._soundsController = this.GetComponent<PlayerSoundsController>();

            this.PreviousTilePositions = new List<Vector3Int>();

            this._deployTransforms[0] = this._leftDeployTransform.GetComponent<Transform>();
            this._deployTransforms[1] = this._rightDeployTransform.GetComponent<Transform>();

            this._deployColliders[0] = this._leftDeployTransform.GetComponent<TrapOverlap>();
            this._deployColliders[1] = this._rightDeployTransform.GetComponent<TrapOverlap>();
        }

        public void SurveyTrapDeployment(bool isGrounded, float direction)
        {
            if (!isGrounded) return;

            // Check whether to deploy left or right
            var deployChecker = direction < 0
                ? this._leftDeployTransform
                : this._rightDeployTransform;
            var deployPosition = deployChecker.GetComponent<Transform>().position;
            var deploymentOrigin = this.TileMap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (this.PreviousTilePositions.Count < 1 || deploymentOrigin != this.PreviousTilePositions[0])
            {
                // The tile changed, so flush the tint on the previous tiles and reset the collision status
                this.ClearTrapDeployment();

                if (this.IsSelectingTileSFX) this._soundsController.OnTileSelectMove();
                else this.IsSelectingTileSFX = !this.IsSelectingTileSFX;

                // Get the grid placement data for the selected prefab
                var selectedTrapPrefab = this.Traps[this.CurrentTrapIndex];
                var prefabPoints = direction < 0
                        ? selectedTrapPrefab.GetLeftGridPoints()
                        : selectedTrapPrefab.GetRightGridPoints();

                // Validate the deployment of the trap with a validation score
                var validationScore = 0;
                var currentCollision = deployChecker.GetComponent<TrapOverlap>().GetCollisionData();

                if (currentCollision)
                {
                    // Simulate a broad phase of collision; if there's something in the general area, check if any of
                    // the tiles to be painted are within the collision bounds
                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        var tileSpacePosition = deploymentOrigin + prefabOffsetPosition;

                        // Validation score is based on if the trap prefab overlaps a specific tile of type TrapTile
                        validationScore = IsTileOfType<TrapTile>(this.TileMap, tileSpacePosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        this.TileMap.SetTileFlags(tileSpacePosition, TileFlags.None);
                        this.PreviousTilePositions.Add(tileSpacePosition);

                        // Check tile collision
                        if (!this.IsColliding) this.CheckTileCollision(currentCollision, tileSpacePosition);
                    }

                    if (this.IsColliding)
                    {
                        this.InvalidateTrapDeployment();
                        return;
                    }
                }
                else
                {
                    this.IsColliding = false;

                    foreach (var prefabOffsetPosition in prefabPoints)
                    {
                        validationScore = IsTileOfType<TrapTile>(this.TileMap, deploymentOrigin + prefabOffsetPosition)
                            ? ++validationScore
                            : validationScore;

                        // Allow to tile to be edited
                        this.TileMap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                        this.PreviousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
                    }
                }

                // If the validation score isn't high enough, paint the selected tiles an invalid color
                if (!selectedTrapPrefab.IsValidScore(validationScore))
                {
                    this.InvalidateTrapDeployment();
                    return;
                }

                this.ValidateTrapDeployment();
            }

            // Check that a trap is not already placed there
            if (this.IsColliding) this.InvalidateTrapDeployment();
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
            var tileWorldPosition = this.TileMap.CellToWorld(tileSpacePosition);

            // Check that the tile unit is not within the collision bounds
            var bounds = currentCollision.bounds;
            var maxBounds = bounds.max;
            var minBounds = bounds.min;

            var vertices1 = (tileWorldPosition.x <= maxBounds.x) && (tileWorldPosition.y <= maxBounds.y);
            var vertices2 = (tileWorldPosition.x >= minBounds.x) && (tileWorldPosition.y >= minBounds.y);

            // If any tile is found within the collider, invalidate the deployment
            if (vertices1 && vertices2) this.IsColliding = true;
        }

        private void InvalidateTrapDeployment()
        {
            // Paint all the tiles red
            foreach (var previousTilePosition in this.PreviousTilePositions)
            {
                this.TileMap.SetColor(previousTilePosition, new Color(1, 0, 0, 0.3f));
            }

            this.CanDeploy = false;
        }

        private void ValidateTrapDeployment()
        {
            // Paint all the tiles green
            foreach (var previousTilePosition in this.PreviousTilePositions)
            {
                this.TileMap.SetColor(previousTilePosition, new Color(0, 1, 0, 0.3f));
            }

            this.CanDeploy = true;
        }

        public void ClearTrapDeployment()
        {
            foreach (var previousTilePosition in this.PreviousTilePositions)
            {
                this.TileMap.SetColor(previousTilePosition, new Color(1, 1, 1, 0));
            }
            this.CanDeploy = false;

            // Clear the data of the previous tile
            this.PreviousTilePositions.Clear();
        }

        //========================================
        // Getters & Setters
        //========================================
        public GameObject GetCurrentTrap()
        {
            return this._trapPrefabs[this.CurrentTrapIndex];
        }

        public int GetCurrentTrapCost()
        {
            return this.Traps[this.CurrentTrapIndex].Cost;
        }
    }
}
