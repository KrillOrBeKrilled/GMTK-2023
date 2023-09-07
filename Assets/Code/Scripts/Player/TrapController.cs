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
namespace Player {
    /// <summary>
    /// Handles the eligibility calculation of tile placement called every frame
    /// through tilemap and custom collision detection. Contains helper methods to check tile
    /// types, validate tiles (paint), invalidate tiles (paint), and clear the deployment grid.
    /// </summary>
    public class TrapController : MonoBehaviour {
        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;

        // ------------- Trap Deployment -------------
        [Tooltip("The trap prefabs to be deployed in the level.")]
        [SerializeField] private List<GameObject> _trapPrefabs;
        
        [Tooltip("The Trap script attached to each trap prefab.")]
        public List<Trap> Traps => this._trapPrefabs.Select(prefab => prefab.GetComponent<Trap>()).ToList();
        [Tooltip("The index of the current selected trap.")]
        public int CurrentTrapIndex { get; private set; }

        [Tooltip("The invisible tilemap for checking trap deployment validity and painting tiles.")]
        public Tilemap TrapTilemap;
        [Tooltip("The level tilemap for the environment and its colliders.")]
        public Tilemap GroundTilemap;
        
        [Tooltip("The canvas to spawn trap UI (e.g. building completion bars).")]
        [SerializeField] private Canvas _trapCanvas;

        private List<Vector3Int> _previousTilePositions;

        [Tooltip("The transform data for the trap deployment position on each side of the player.")]
        [SerializeField] private Transform _leftDeployTransform, _rightDeployTransform;
        private readonly Transform[] _deployTransforms = new Transform[2];

        private bool _isColliding, _isSelectingTileSFX, _canDeploy;

        private void Awake() {
            this._soundsController = this.GetComponent<PlayerSoundsController>();

            this._previousTilePositions = new List<Vector3Int>();

            this._deployTransforms[0] = this._leftDeployTransform.GetComponent<Transform>();
            this._deployTransforms[1] = this._rightDeployTransform.GetComponent<Transform>();
        }

        /// <summary>
        /// Checks whether the current selected trap can be deployed or not and paints the trap tilemap tiles
        /// accordingly. Plays SFX associated with the movement of the trap tile selection.
        /// </summary>
        /// <param name="isGrounded"> If the player is currently touching the ground. </param>
        /// <param name="direction"> The direction for the character to check trap deployment validity. </param>
        /// <remarks> Trap deployment cannot be validated when the player is flying or the selected trap
        /// does not overlap enough custom <see cref="TrapTile">TrapTiles</see> to reach the
        /// <see cref="Trap.ValidationScore"/> threshold. If the player is flying, the grid will not be
        /// painted.
        /// <para> The painting of the trap tilemap tiles is decided by the player direction to use the
        /// <see cref="_leftDeployTransform"/> or <see cref="_rightDeployTransform"/> as the origin, with the
        /// <see cref="Trap.LeftGridPoints"/> and <see cref="Trap.RightGridPoints"/> data as tile offsets to
        /// accurately pinpoint the trap tiles in question. </para> </remarks>
        public void SurveyTrapDeployment(bool isGrounded, float direction) {
            if (!isGrounded) 
                return;

            // Check whether to deploy left or right
            var deployPosition = direction < 0
                ? this._leftDeployTransform.position
                : this._rightDeployTransform.position;
            var deploymentOrigin = this.TrapTilemap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (this._previousTilePositions.Count >= 1 && deploymentOrigin == this._previousTilePositions[0]) 
                return;

            // The tile changed, so flush the tint on the previous tiles and reset the collision status
            this.ClearTrapDeployment();

            if (this._isSelectingTileSFX) 
                this._soundsController.OnTileSelectMove();
            else 
                this._isSelectingTileSFX = !this._isSelectingTileSFX;

            // Get the grid placement data for the selected prefab
            var selectedTrapPrefab = this.Traps[CurrentTrapIndex];
            var prefabPoints = direction < 0
                ? selectedTrapPrefab.GetLeftGridPoints()
                : selectedTrapPrefab.GetRightGridPoints();

            // Validate the deployment of the trap with a validation score
            var validationScore = 0;

            foreach (var prefabOffsetPosition in prefabPoints) {
                validationScore = IsTileOfType<TrapTile>(this.TrapTilemap, deploymentOrigin + prefabOffsetPosition)
                    ? ++validationScore
                    : validationScore;

                // Allow to tile to be edited
                this.TrapTilemap.SetTileFlags(deploymentOrigin + prefabOffsetPosition, TileFlags.None);
                this._previousTilePositions.Add(deploymentOrigin + prefabOffsetPosition);
            }

            // If the validation score isn't high enough, paint the selected tiles an invalid color
            if (!selectedTrapPrefab.IsValidScore(validationScore)) 
                this.InvalidateTrapDeployment();
            else 
                this.ValidateTrapDeployment();
        }

        /// <summary>
        /// Deploys the current selected trap with associated SFX, provided that <see cref="_canDeploy"/>
        /// is toggled and the trap cost is affordable.
        /// </summary>
        /// <param name="playerDirection"> The direction for the player to deploy the trap. </param>
        /// <param name="trapIndex"> Stores the index of the trap when it is deployed. If trap deployment
        /// is invalid, this parameter remains unchanged. </param>
        /// <returns> If the trap is successfully deployed. </returns>
        /// <remarks> Trap deployment ends with the instantiation of the trap prefab corresponding to the
        /// <see cref="trapIndex"/>. </remarks>
        public bool DeployTrap(float playerDirection, out int trapIndex) {
            trapIndex = this.CurrentTrapIndex;
            
            // Left out of State pattern to allow this during movement
            if(!this._canDeploy || this._previousTilePositions.Count < 1) {
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

        /// <summary> Helper method for clearing the painted trap tilemap tiles and disabling trap surveying SFX. </summary>
        public void DisableTrapDeployment() {
            ClearTrapDeployment();
            this._isSelectingTileSFX = false;
        }

        /// <summary> Sets the <see cref="CurrentTrapIndex"/> and resets the painted trap tilemap tiles. </summary>
        /// <param name="trapIndex"> The index of the trap to be selected. </param>
        public void ChangeTrap(int trapIndex) {
            this.CurrentTrapIndex = trapIndex;
            DisableTrapDeployment();
        }

        //========================================
        // Helper Methods
        //========================================

        /// <summary> Helper method for comparing a tilemap tile to a target tile type. </summary>
        /// <param name="tilemap"> The tilemap used to locate the tile in question. </param>
        /// <param name="position"> The tilemap position used to locate the tile in question. </param>
        /// <typeparam name="T"> The target tile type. </typeparam>
        /// <returns> If the tile in question is the same type as the target type. </returns>
        private static bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase {
            var targetTile = tilemap.GetTile(position);
            return targetTile is T;
        }

        /// <summary>
        /// Resets trap deployment validity and paints the selected tiles in the tilemap a rejection color.
        /// </summary>
        private void InvalidateTrapDeployment() {
            TilemapManager.Instance.PaintTilesRejectionColor(_previousTilePositions);
            this._canDeploy = false;
        }

        /// <summary>
        /// Toggles trap deployment validity and paints the selected tiles in the tilemap a success color.
        /// </summary>
        private void ValidateTrapDeployment() {
            TilemapManager.Instance.PaintTilesConfirmationColor(_previousTilePositions);
            this._canDeploy = true;
        }

        /// <summary>
        /// Resets trap deployment validity, clears the stored tilemap tile locations data, and paints the
        /// selected tiles blank.
        /// </summary>
        private void ClearTrapDeployment() {
            TilemapManager.Instance.PaintTilesBlank(_previousTilePositions);
            this._canDeploy = false;
        
            // Clear the data of the previous tile
            this._previousTilePositions.Clear();
        }
        
        //========================================
        // Ground tiles
        //========================================
        
        /// <summary>
        /// Locates a tilemap tile from a world space position and checks that it's a
        /// <see cref="CustomGroundRuleTile"/>.
        /// </summary>
        /// <param name="position"> The world space position used to locate the tile in question. </param>
        /// <returns> If the tile in question is a <see cref="CustomGroundRuleTile"/>. </returns>
        public bool CheckForGroundTile(Vector3 position) {
            var contactTilePosition = this.GroundTilemap.WorldToCell(position);
            
            return IsTileOfType<CustomGroundRuleTile>(this.GroundTilemap, contactTilePosition);
        }

        //========================================
        // Getters & Setters
        //========================================
        
        /// <summary>
        /// Retrieves the cost of the current selected trap.
        /// </summary>
        /// <returns> The cost of the current selected trap. </returns>
        public int GetCurrentTrapCost() {
            return this.Traps[this.CurrentTrapIndex].Cost;
        }
    }
}
