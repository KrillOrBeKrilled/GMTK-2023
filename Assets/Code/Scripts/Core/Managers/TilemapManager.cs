using System.Collections.Generic;
using KrillOrBeKrilled.Extensions;
using KrillOrBeKrilled.Player;
using KrillOrBeKrilled.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// TilemapManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// Handles all logic that directly alters the tilemaps crucial to the tile-based
    /// gameplay.
    /// </summary>
    /// <remarks>
    /// Use cases include the visual representation of the ability or lack
    /// of to deploy traps in the current position, replacing or setting TrapTile types
    /// to revoke or reset validation scores in the PlayerController class, and placing
    /// or removing tiles in the level tilemap to dynamically alter the environment.
    /// </remarks>
    public class TilemapManager : Singleton<TilemapManager> {
        // ----------------- Tilemaps ----------------
        [Header("Tilemaps")] 
        [SerializeField] private Tilemap _trapTileMap;

        // -------------- Tiles to Paint -------------
        [Header("Painting Tiles")] 
        [SerializeField] private TileBase _blankTile;
        [SerializeField] private TileBase _trapValidationTile;

        // ------------- Painting Colors -------------
        [Header("Painting Colors")] 
        public Color ConfirmationColor, RejectionColor;
        
        private Tilemap _levelTileMap;
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region Set Tiles
        
        /// <summary>
        /// Removes the tiles at the tile positions in the level tilemap and invalidates the same tile positions
        /// in the trap tilemap.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void ClearLevelTiles(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                this._levelTileMap.SetTile(position, null);

                // Clear the TrapTile types to also prevent traps from being rebuilt here
                this._trapTileMap.SetTile(position, this._blankTile);

                // Set the new tile the invalid color
                this._trapTileMap.SetTileFlags(position, TileFlags.None);
                this._trapTileMap.SetColor(position, this.RejectionColor);
            }
        }
        
        /// <summary>
        /// Resets the tiles at the tile positions in the trap tilemap to enable the building of traps in such
        /// positions.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        /// <remarks>
        /// When a trap is destroyed it invokes a UnityAction. The TrapController listens for this change and
        /// tries to reset trap tiles. This causes null pointer exception when a level ends (tilemaps are
        /// destroyed) and this method continues to try resetting tilemaps. So we check if the tilemap is null
        /// before trying to update them.
        /// </remarks>
        public void ResetTrapTiles(IEnumerable<Vector3Int> tilePositions) {
            if (this._trapTileMap == null) {
                Debug.LogWarning("ResetTrapTiles: Tilemap is null");
                return;
            }
            
            foreach (var position in tilePositions) {
                this._trapTileMap.SetTile(position, this._trapValidationTile);

                // Mark the new tile as editable
                this._trapTileMap.SetTileFlags(position, TileFlags.None);
            }
        }
        
        #endregion
        
        #region Paint Tiles
        
        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap transparent.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesBlank(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                this._trapTileMap.SetColor(position, new Color(1, 1, 1, 0));
            }
        }
        
        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap the <see cref="ConfirmationColor"/>.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesConfirmationColor(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                this._trapTileMap.SetColor(position, this.ConfirmationColor);
            }
        }

        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap the <see cref="RejectionColor"/>.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesRejectionColor(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                this._trapTileMap.SetColor(position, this.RejectionColor);
            }
        }
        
        #endregion

        /// <summary>
        /// Sets up subscriptions to player events required for proper execution.
        /// </summary>
        /// <param name="levelTilemap"> The tilemap consisting of tiles forming the game level. </param>
        /// <param name="trapController"> Provides all tilemap-related events to be executed in response to player
        /// input actions. </param>
        /// <param name="playerCharacter"> <see cref="PlayerCharacter"/> used to update the respawn position of
        /// the player. </param>
        public void Initialize(Tilemap levelTilemap, TrapController trapController, PlayerCharacter playerCharacter) {
            this._levelTileMap = levelTilemap;
            trapController.OnPaintTiles.AddListener(this.OnPaintTiles);
            playerCharacter.SetGetFallRespawnPos(this.GetGroundTileBelowPos);
        }

        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Searches for the tile of type <see cref="CustomGroundRuleTile"/> below the provided
        /// <paramref name="pos"/> up to <paramref name="limit"/> tiles below.
        /// </summary>
        /// <param name="pos"> The position below which to search for the tile. </param>
        /// <param name="limit"> The limit of how many tiles to check. </param>
        /// <returns></returns>
        private Vector3? GetGroundTileBelowPos(Vector3 pos, int limit) {
            int offset = 0;
            Vector3Int coordinate = pos.RoundToInt();
            bool found = false;
            while (offset < limit) {
                Vector3Int offsetPos = coordinate - new Vector3Int(0, offset, 0);
                CustomGroundRuleTile tile = this._levelTileMap.GetTile(offsetPos) as CustomGroundRuleTile;
                
                if (tile != null) {
                    found = true;
                    break;
                }

                offset++;
            }
            
            if (!found) {
                return null;
            }

            return this._levelTileMap.CellToWorld(coordinate);
        }

        /// <summary>
        /// Selects an operation to act on the provided tilemap positions based on the specified
        /// <see cref="TrapController.PaintMode"/>.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="TrapController.OnPaintTiles"/> event. </remarks>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        /// <param name="paintMode"> Specifies the operation type to be performed on the tilemap tiles. </param>
        private void OnPaintTiles(IEnumerable<Vector3Int> tilePositions, TrapController.PaintMode paintMode) {
            switch (paintMode) {
                case TrapController.PaintMode.FreeTile:
                    this.ResetTrapTiles(tilePositions);
                    break;
                case TrapController.PaintMode.AllocateTileSpace:
                    this.ClearLevelTiles(tilePositions);
                    break;
                case TrapController.PaintMode.PaintBlank:
                    this.PaintTilesBlank(tilePositions);
                    break;
                case TrapController.PaintMode.PaintInvalid:
                    this.PaintTilesRejectionColor(tilePositions);
                    break;
                case TrapController.PaintMode.PaintValid:
                    this.PaintTilesConfirmationColor(tilePositions);
                    break;
                default:
                    Debug.LogError("Invalid paint mode received by TilemapManager! Check " +
                                   "TrapController.OnPaintTiles UnityEvent for potential illegal usage.");
                    break;
            }
        }
        
        #endregion
    }
}