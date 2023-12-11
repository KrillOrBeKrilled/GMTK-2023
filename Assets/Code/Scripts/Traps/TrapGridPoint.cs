using System;
using UnityEngine;

//*******************************************************************************************
// TrapGridPoint
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Stores data associated with a grid point required to be available in order to
    /// build a trap. 
    /// </summary>
    /// <remarks>
    /// Includes data on the designated position and specifications of its required
    /// tile type.
    /// </remarks>
    [Serializable]
    public class TrapGridPoint {
        [Tooltip("Tilemap position offset to specify the tile position needed for deployment of this trap calculated " +
                 "from an origin in the TrapController.")]
        public Vector3Int GridPosition;
        [Tooltip("If this tile position must be of the TrapTile type.")]
        public bool IsTrapTile;
    }
}
