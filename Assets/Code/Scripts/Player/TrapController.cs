using Audio;
using Managers;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using Traps;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// TrapController
//*******************************************************************************************
namespace Player
{
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
        public int CurrentTrapIndex { get; private set; }

        public Tilemap TrapTilemap;
        public Tilemap GroundTilemap;
        
        // The canvas to spawn trap UI
        [SerializeField] private Canvas _trapCanvas;

        private List<Vector3Int> _previousTilePositions;

        [SerializeField] private Transform _leftDeployTransform, _rightDeployTransform;
        private readonly Transform[] _deployTransforms = new Transform[2];

        private bool _isColliding, _isSelectingTileSFX, _canDeploy;

        private void Awake()
        {
            this._soundsController = this.GetComponent<PlayerSoundsController>();

            this._previousTilePositions = new List<Vector3Int>();

            this._deployTransforms[0] = this._leftDeployTransform.GetComponent<Transform>();
            this._deployTransforms[1] = this._rightDeployTransform.GetComponent<Transform>();
        }

        public void SurveyTrapDeployment(bool isGrounded, float direction)
        {
            if (!isGrounded) return;

            // Check whether to deploy left or right
            var deployPosition = direction < 0
                ? this._leftDeployTransform.position
                : this._rightDeployTransform.position;
            var deploymentOrigin = this.TrapTilemap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (this._previousTilePositions.Count >= 1 && deploymentOrigin == this._previousTilePositions[0]) return;

            // The tile changed, so flush the tint on the previous tiles and reset the collision status
            this.ClearTrapDeployment();

            if (this._isSelectingTileSFX) this._soundsController.OnTileSelectMove();
            else this._isSelectingTileSFX = !this._isSelectingTileSFX;

            // Get the grid placement data for the selected prefab
            var selectedTrapPrefab = this.Traps[CurrentTrapIndex];
            var prefabPoints = direction < 0
                ? selectedTrapPrefab.GetLeftGridPoints()
                : selectedTrapPrefab.GetRightGridPoints();

            // Validate the deployment of the trap with a validation score
            var validationScore = 0;

            foreach (var prefabOffsetPosition in prefabPoints)
            {
                validationScore = IsTileOfType<TrapTile>(this.TrapTilemap, deploymentOrigin + prefabOffsetPosition)
                    ? ++validationScore
                    : validationScore;

                // Allow to tile to be edited
                this.TrapTilemap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                this._previousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
            }

            // If the validation score isn't high enough, paint the selected tiles an invalid color
            if (!selectedTrapPrefab.IsValidScore(validationScore)) this.InvalidateTrapDeployment();
            else this.ValidateTrapDeployment();
        }

        public bool DeployTrap(float playerDirection, out int trapIndex)
        {
            trapIndex = this.CurrentTrapIndex;
            
            // Left out of State pattern to allow this during movement
            if(!this._canDeploy || this._previousTilePositions.Count < 1)
            {
                // TODO: Make an animation for this!
                print("Can't Deploy Trap!");
                return false;
            }

            var trapToSpawn = this._trapPrefabs[this.CurrentTrapIndex];
            var trapScript = this.Traps[this.CurrentTrapIndex];
            
            if (!CoinManager.Instance.CanAfford(trapScript.Cost)) {
                print("Can't afford the trap!");
                return false;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = this.TrapTilemap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = playerDirection < 0
                ? trapScript.GetLeftSpawnPoint(deploymentOrigin)
                : trapScript.GetRightSpawnPoint(deploymentOrigin);

            var trapGameObject = Instantiate(trapToSpawn.gameObject);
            trapGameObject.GetComponent<Trap>().Construct(spawnPosition, this._trapCanvas, 
                this._previousTilePositions.ToArray(), this._soundsController);
            this._isColliding = true;

            CoinManager.Instance.ConsumeCoins(trapScript.Cost);
            this._soundsController.OnTileSelectConfirm();

            return true;
        }

        public void DisableTrapDeployment()
        {
            ClearTrapDeployment();
            this._isSelectingTileSFX = false;
        }

        public void ChangeTrap(int trapIndex)
        {
            this.CurrentTrapIndex = trapIndex;
            DisableTrapDeployment();
        }

        //========================================
        // Helper Methods
        //========================================

        private static bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase
        {
            var targetTile = tilemap.GetTile(position);
            return targetTile is T;
        }

        private void InvalidateTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesRejectionColor(_previousTilePositions);
            this._canDeploy = false;
        }

        private void ValidateTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesConfirmationColor(_previousTilePositions);
            this._canDeploy = true;
        }

        private void ClearTrapDeployment()
        {
            TilemapManager.Instance.PaintTilesBlank(_previousTilePositions);
            this._canDeploy = false;
        
            // Clear the data of the previous tile
            this._previousTilePositions.Clear();
        }
        
        //========================================
        // Ground tiles
        //========================================
        public bool CheckForGroundTile(Vector3 position)
        {
            var contactTilePosition = this.GroundTilemap.WorldToCell(position);
            
            return IsTileOfType<CustomGroundRuleTile>(this.GroundTilemap, contactTilePosition);
        }

        //========================================
        // Getters & Setters
        //========================================
        public int GetCurrentTrapCost()
        {
            return this.Traps[this.CurrentTrapIndex].Cost;
        }
    }
}
