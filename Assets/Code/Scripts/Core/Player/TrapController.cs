using System.Collections.Generic;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Tiles;
using KrillOrBeKrilled.Traps;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// TrapController
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Handles the eligibility calculation of tile placement called every frame
    /// through tilemap and custom collision detection. Contains helper methods to check tile
    /// types, validate tiles (paint), invalidate tiles (paint), and clear the deployment grid.
    /// </summary>
    public class TrapController : MonoBehaviour {
        // ------------- Sound Effects ---------------
        private PlayerSoundsController _soundsController;
        private TrapSoundsController _trapSoundsController;

        // ------------- Trap Deployment -------------
        public ReadOnlyCollection<Trap> Traps;
        public Trap CurrentTrap { get; private set; }

        [Tooltip("The trap prefabs to be deployed in the level.")]
        [SerializeField] private List<Trap> _trapPrefabs;

        [Tooltip("The invisible tilemap for checking trap deployment validity and painting tiles.")]
        [SerializeField] internal Tilemap TrapTilemap;
        [Tooltip("The level tilemap for the environment and its colliders.")]
        [SerializeField] internal Tilemap GroundTilemap;

        [Tooltip("The canvas to spawn trap UI (e.g. building completion bars).")]
        [SerializeField] private Canvas _trapCanvas;

        private List<Vector3Int> _previousTilePositions;

        [Tooltip("The transform data for the trap deployment position on each side of the player.")]
        [SerializeField] private Vector3 _leftDeployPosition, _rightDeployPosition;

        private bool _isSelectingTileSFX, _canDeploy;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            TryGetComponent(out this._soundsController);
            TryGetComponent(out this._trapSoundsController);

            this._previousTilePositions = new List<Vector3Int>();
            this.Traps = this._trapPrefabs.AsReadOnly();
            this.CurrentTrap = this._trapPrefabs.First();
        }

        #endregion

        //========================================
        // Internal Methods
        //========================================

        #region Internal Methods

        #region Trap Deployment

        /// <summary>
        /// Sets the <see cref="CurrentTrap"/> and resets the painted trap tilemap tiles.
        /// </summary>
        /// <param name="trap">Trap to be selected.</param>
        internal void ChangeTrap(Trap trap) {
            this.CurrentTrap = trap;
            this.DisableTrapDeployment();
        }

        /// <summary>
        /// Deploys the current selected trap with associated SFX, provided that <see cref="_canDeploy"/>
        /// is toggled and the trap cost is affordable.
        /// </summary>
        /// <param name="playerDirection"> The direction for the player to deploy the trap. </param>
        /// <param name="trap"> Stores the trap when it is deployed. If trap deployment
        /// is invalid, this parameter remains unchanged. </param>
        /// <returns> If the trap is successfully deployed. </returns>
        /// <remarks>
        /// Trap deployment ends with the instantiation of the trap prefab corresponding to the
        /// <see cref="CurrentTrap"/>.
        /// </remarks>
        internal bool DeployTrap(float playerDirection, out Trap trap) {
            trap = this.CurrentTrap;

            // Left out of State pattern to allow this during movement
            if(!this._canDeploy || this._previousTilePositions.Count < 1) {
                // TODO: Make an animation for this!
                return false;
            }

            if (!ResourceManager.Instance.CanAffordCost(this.CurrentTrap.Recipe)) {
                return false;
            }

            // Convert the origin tile position to world space
            var deploymentOrigin = this.TrapTilemap.CellToWorld(_previousTilePositions[0]);
            var spawnPosition = playerDirection < 0
                ? this.CurrentTrap.GetLeftSpawnPoint(deploymentOrigin)
                : this.CurrentTrap.GetRightSpawnPoint(deploymentOrigin);

            Trap spawnedTrap = Instantiate(this.CurrentTrap);
            Vector3Int[] tilePositionsCopy = this._previousTilePositions.ToArray();
            spawnedTrap.Construct(spawnPosition, this._trapCanvas, this._previousTilePositions.ToArray(), _trapSoundsController, 
                () => TilemapManager.Instance.ResetTrapTiles(tilePositionsCopy));
            
            // Delete/invalidate all the tiles overlapping the trap
            TilemapManager.Instance.ClearLevelTiles(this._previousTilePositions.ToArray());

            ResourceManager.Instance.ConsumeResources(this.CurrentTrap.Recipe);
            this._soundsController.OnTileSelectConfirm();

            return true;
        }

        /// <summary>
        /// Helper method for clearing the painted trap tilemap tiles and disabling trap surveying SFX.
        /// </summary>
        internal void DisableTrapDeployment() {
            ClearTrapDeployment();
            this._isSelectingTileSFX = false;
        }

        /// <summary>
        /// Checks whether the current selected trap can be deployed or not and paints the trap tilemap tiles
        /// accordingly. Plays SFX associated with the movement of the trap tile selection.
        /// </summary>
        /// <param name="isGrounded"> If the player is currently touching the ground. </param>
        /// <param name="direction"> The direction for the character to check trap deployment validity. </param>
        /// <remarks>
        /// Trap deployment cannot be validated when the player is flying or the selected trap does not overlap
        /// enough custom <see cref="TrapTile">TrapTiles</see> to reach the <see cref="Trap.ValidationScore"/>
        /// threshold. If the player is flying, the grid will not be painted.
        /// <para> The painting of the trap tilemap tiles is decided by the player direction to use the
        /// <see cref="_leftDeployTransform"/> or <see cref="_rightDeployTransform"/> as the origin, with the
        /// <see cref="Trap.LeftGridPoints"/> and <see cref="Trap.RightGridPoints"/> data as tile offsets to
        /// accurately pinpoint the trap tiles in question. </para>
        /// </remarks>
        internal void SurveyTrapDeployment(bool isGrounded, float direction) {
            if (!isGrounded) {
                return;
            }

            // Check whether to deploy left or right
            var deployPosition = direction < 0
                ? this.transform.position + this._leftDeployPosition
                : this.transform.position + this._rightDeployPosition;
            var deploymentOrigin = this.TrapTilemap.WorldToCell(deployPosition);

            // Ensure that there are no query results yet or that the deploymentOrigin has changed
            if (this._previousTilePositions.Count >= 1 && deploymentOrigin == this._previousTilePositions[0]) {
                return;
            }

            // The tile changed, so flush the tint on the previous tiles and reset the collision status
            this.ClearTrapDeployment();

            if (this._isSelectingTileSFX) {
                this._soundsController.OnTileSelectMove();
            } else {
                this._isSelectingTileSFX = !this._isSelectingTileSFX;
            }

            // Get the grid placement data for the selected prefab
            var prefabPoints = direction < 0
                ? this.CurrentTrap.GetLeftGridPoints()
                : this.CurrentTrap.GetRightGridPoints();

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
            if (!this.CurrentTrap.IsValidScore(validationScore)) {
                this.InvalidateTrapDeployment();
            } else {
                this.ValidateTrapDeployment();
            }
        }

        #endregion

        /// <summary>
        /// Locates a tilemap tile from a world space position and checks that it's a <see cref="CustomGroundRuleTile"/>.
        /// </summary>
        /// <param name="position"> The world space position used to locate the tile in question. </param>
        /// <returns> If the tile in question is a <see cref="CustomGroundRuleTile"/>. </returns>
        internal bool CheckForGroundTile(Vector3 position) {
            var contactTilePosition = this.GroundTilemap.WorldToCell(position);

            return IsTileOfType<CustomGroundRuleTile>(this.GroundTilemap, contactTilePosition);
        }

        /// <summary>
        /// Retrieves the resource recipe of the current selected trap.
        /// </summary>
        /// <returns> The dictionary representing the recipe of the current selected trap. </returns>
        internal Dictionary<ResourceType, int> GetCurrentTrapCost() {
            return this.CurrentTrap.Recipe;
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

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

        /// <summary>
        /// Resets trap deployment validity and paints the selected tiles in the tilemap a rejection color.
        /// </summary>
        private void InvalidateTrapDeployment() {
            TilemapManager.Instance.PaintTilesRejectionColor(_previousTilePositions);
            this._canDeploy = false;
        }

        /// <summary>
        /// Helper method for comparing a tilemap tile to a target tile type.
        /// </summary>
        /// <param name="tilemap"> The tilemap used to locate the tile in question. </param>
        /// <param name="position"> The tilemap position used to locate the tile in question. </param>
        /// <typeparam name="T"> The target tile type. </typeparam>
        /// <returns> If the tile in question is the same type as the target type. </returns>
        private static bool IsTileOfType<T>(ITilemap tilemap, Vector3Int position) where T : TileBase {
            var targetTile = tilemap.GetTile(position);
            return targetTile is T;
        }

        /// <summary>
        /// Toggles trap deployment validity and paints the selected tiles in the tilemap a success color.
        /// </summary>
        private void ValidateTrapDeployment() {
            TilemapManager.Instance.PaintTilesConfirmationColor(_previousTilePositions);
            this._canDeploy = true;
        }

        #endregion
    }
}
