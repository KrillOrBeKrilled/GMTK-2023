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
        public Vector3Int GridPosition;
        public bool IsTrapTile;
    }
}
