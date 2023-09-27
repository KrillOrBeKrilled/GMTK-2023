using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// CustomTrapRuleTile
//*******************************************************************************************
namespace KrillOrBeKrilled.Tiles {
    /// <summary>
    /// A subclass of <see cref="RuleTile"/> that extends the functionality of the Null
    /// rule to pass if any of the specified tiles are connected as well as its own type.
    /// Includes extra rules for checking ground and dungeon neighboring tiles.
    /// </summary>
    [CreateAssetMenu(menuName = "2D/Custom Tiles/Custom Trap Rule Tile")]
    public class CustomTrapRuleTile : RuleTile<CustomTrapRuleTile.Neighbor> {
        [Tooltip("Neighboring tile types that can be considered as Null.")]
        [SerializeField] internal TileBase[] tileTypesToConnect;

        [Tooltip("Neighboring tile types that can be considered as Ground.")]
        [SerializeField] internal TileBase[] groundTileTypes;

        [Tooltip("Neighboring tile types that can be considered as Dungeon.")]
        [SerializeField] internal TileBase[] dungeonTileTypes;

        public class Neighbor : RuleTile.TilingRule.Neighbor {
            public const int Null = 1;
            public const int NotNull = 2;
            public const int Ground = 3;
            public const int Dungeon = 4;
        }
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        public override bool RuleMatch(int neighbor, TileBase tile) {
            switch (neighbor) {
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
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        #region Rule Match
        
        /// <summary>
        /// Checks that the neighboring tile is contained within <see cref="dungeonTileTypes"/>.
        /// </summary>
        /// <param name="tile"> The neighboring tile in question. </param>
        /// <returns> If the neighboring tile is contained in <see cref="dungeonTileTypes"/>. </returns>
        private bool CheckDungeon(TileBase tile) {
            return dungeonTileTypes.Contains(tile);
        }

        /// <summary>
        /// Checks that the neighboring tile is contained within <see cref="groundTileTypes"/>.
        /// </summary>
        /// <param name="tile"> The neighboring tile in question. </param>
        /// <returns> If the neighboring tile is contained in <see cref="groundTileTypes"/>. </returns>
        private bool CheckGround(TileBase tile) {
            return groundTileTypes.Contains(tile);
        }
        
        /// <summary>
        /// Checks that the neighboring tile is the same type as this tile or is contained within
        /// <see cref="tileTypesToConnect"/>.
        /// </summary>
        /// <param name="tile"> The neighboring tile in question. </param>
        /// <returns>
        /// If the neighboring tile is the same type as this tile or contained in <see cref="tileTypesToConnect"/>.
        /// </returns>
        private bool CheckThis(TileBase tile) {
            return tileTypesToConnect.Contains(tile) || tile == this;
        }

        /// <summary> Checks that the neighboring tile is not the same type as this tile. </summary>
        /// <param name="tile"> The neighboring tile in question. </param>
        /// <returns> If the neighboring tile is the not same type as this tile. </returns>
        private bool CheckNotThis(TileBase tile) {
            return tile != this;
        }
        
        #endregion
        
        #endregion
    }
}