using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// TilemapManager
//*******************************************************************************************
/// <summary>
/// A singleton manager class to handle all logic that directly alters the tilemaps
/// crucial to the tile-based gameplay. Use cases include the visual representation
/// of the ability or lack of one to deploy traps in the current position, replacing
/// or setting TrapTile types to revoke or reset validation scores in the PlayerController
/// class, and placing or removing tiles in the level tilemap to dynamically alter the
/// environment.
/// </summary>
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
    
    public void PaintTilesConfirmationColor(IEnumerable<Vector3Int> tilePositions)
    {
        foreach (var position in tilePositions)
        {
            _trapTileMap.SetColor(position, ConfirmationColor);
        }
    }
    
    public void PaintTilesRejectionColor(IEnumerable<Vector3Int> tilePositions)
    {
        foreach (var position in tilePositions)
        {
            _trapTileMap.SetColor(position, RejectionColor);
        }
    }

    public void PaintTilesBlank(IEnumerable<Vector3Int> tilePositions)
    {
        foreach (var position in tilePositions)
        {
            _trapTileMap.SetColor(position, new Color(1, 1, 1, 0));
        }
    }
        
    // ----------- Clearing Tiles ------------
    public void ClearLevelTiles(IEnumerable<Vector3Int> tilePositions)
    {
        foreach (var position in tilePositions)
        {
            _levelTileMap.SetTile(position, null);
                
            // Clear the TrapTile types to also prevent traps from being rebuilt here
            _trapTileMap.SetTile(position, _blankTile);
            
            // Set the new tile the invalid color
            _trapTileMap.SetTileFlags(position, TileFlags.None);
            _trapTileMap.SetColor(position, RejectionColor);
        }
    }
}
