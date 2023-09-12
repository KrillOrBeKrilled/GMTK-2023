using KrillOrBeKrilled.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// TilemapManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Handles all logic that directly alters the tilemaps crucial to the tile-based
    /// gameplay.
    /// </summary>
    /// <remarks> Use cases include the visual representation of the ability or lack
    /// of to deploy traps in the current position, replacing or setting TrapTile types
    /// to revoke or reset validation scores in the PlayerController class, and placing
    /// or removing tiles in the level tilemap to dynamically alter the environment. </remarks>
    public class TilemapManager : Singleton<TilemapManager> {
        // ----------------- Tilemaps ----------------
        [Header("Tilemaps")] 
        [SerializeField] private Tilemap _trapTileMap;
        [SerializeField] private Tilemap _levelTileMap;

        // -------------- Tiles to Paint -------------
        [Header("Painting Tiles")] 
        [SerializeField] private TileBase _blankTile;
        [SerializeField] private TileBase _trapValidationTile;

        // ------------- Painting Colors -------------
        [Header("Painting Colors")] 
        public Color ConfirmationColor, RejectionColor;

        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap the <see cref="ConfirmationColor"/>.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesConfirmationColor(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                _trapTileMap.SetColor(position, ConfirmationColor);
            }
        }

        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap the <see cref="RejectionColor"/>.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesRejectionColor(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                _trapTileMap.SetColor(position, RejectionColor);
            }
        }

        /// <summary>
        /// Paints the tiles at the tile positions in the trap tilemap transparent.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void PaintTilesBlank(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                _trapTileMap.SetColor(position, new Color(1, 1, 1, 0));
            }
        }

        //========================================
        // Setting Tiles
        //========================================
        
        /// <summary>
        /// Removes the tiles at the tile positions in the level tilemap and invalidates the same tile positions
        /// in the trap tilemap.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void ClearLevelTiles(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                _levelTileMap.SetTile(position, null);

                // Clear the TrapTile types to also prevent traps from being rebuilt here
                _trapTileMap.SetTile(position, _blankTile);

                // Set the new tile the invalid color
                _trapTileMap.SetTileFlags(position, TileFlags.None);
                _trapTileMap.SetColor(position, RejectionColor);
            }
        }
        
        /// <summary>
        /// Resets the tiles at the tile positions in the trap tilemap to enable the building of traps in such
        /// positions.
        /// </summary>
        /// <param name="tilePositions"> A list of tile positions corresponding to a tilemap to paint. </param>
        public void ResetTrapTiles(IEnumerable<Vector3Int> tilePositions) {
            foreach (var position in tilePositions) {
                _trapTileMap.SetTile(position, _trapValidationTile);

                // Mark the new tile as editable
                _trapTileMap.SetTileFlags(position, TileFlags.None);
            }
        }
    }
}