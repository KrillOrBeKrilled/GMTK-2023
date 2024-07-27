using System;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
    //*******************************************************************************************
    // TrapIconData
    //*******************************************************************************************
    /// <summary>
    /// Stores data of all <see cref="TrapIcon"/> used to represent every trap in the system.
    /// </summary>
    [CreateAssetMenu(fileName = "TrapIconData", menuName = "Model/TrapIconData")]
    public class TrapIconData : ScriptableObject {
        [SerializeField] private List<TrapIcon> _icons;

        /// <summary>
        /// Finds the trapType icon associated with the provided <see cref="Trap"/> data.
        /// </summary>
        /// <param name="trapType"> The <see cref="Trap"/> class used to determine the trapType type to find the related icon. </param>
        /// <returns> The trapType icon sprite associated with the provided trapType data. </returns>
        public Sprite TrapToImage(TrapType trapType) {
            return this._icons.Find(trapIcon => trapIcon.TrapType == trapType).Icon;
        }
    }

    //*******************************************************************************************
    // TrapIcon
    //*******************************************************************************************
    /// <summary>
    /// Stores data that specifies a trap name to an icon representation.
    /// </summary>
    [Serializable]
    public class TrapIcon {
        public TrapType TrapType;
        public Sprite Icon;
    }
}
