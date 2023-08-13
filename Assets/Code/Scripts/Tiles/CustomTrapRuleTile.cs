using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// CustomTrapRuleTile
//*******************************************************************************************
/// <summary>
/// A subclass of RuleTile that extends the functionality of the Null rule to
/// pass if any of the specified tiles are connected as well as its own type.
/// Includes extra rules for checking ground and dungeon neighboring tiles.
/// </summary>
[CreateAssetMenu(menuName = "2D/Custom Tiles/Custom Trap Rule Tile")]
public class CustomTrapRuleTile : RuleTile<CustomTrapRuleTile.Neighbor> {
    
    public TileBase[] tileTypesToConnect;
    public TileBase[] groundTileTypes;
    public TileBase[] dungeonTileTypes;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 1;
        public const int NotNull = 2;
        public const int Ground = 3;
        public const int Dungeon = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Null:
                return CheckThis(tile);
            case Neighbor.NotNull:
                return CheckNotThis(tile);
            case Neighbor.Ground:
                return CheckGround(tile);
            case Neighbor.Dungeon:
                return CheckDungeon(tile);
        }

        return base.RuleMatch(neighbor, tile);
    }
    
    // --------- Rule Match Helpers ----------
    private bool CheckThis(TileBase tile)
    {
        return tileTypesToConnect.Contains(tile) || tile == this;
    }
    
    private bool CheckNotThis(TileBase tile)
    {
        return tile != this;
    }
    
    private bool CheckDungeon(TileBase tile)
    {
        return dungeonTileTypes.Contains(tile);
    }
    
    private bool CheckGround(TileBase tile)
    {
        return groundTileTypes.Contains(tile);
    }
}