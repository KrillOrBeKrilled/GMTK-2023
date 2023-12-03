using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KrillOrBeKrilled.Traps {
    //*******************************************************************************************
    // TrapIconData
    //*******************************************************************************************
    /// <summary>
    /// Stores data of all <see cref="TrapIcon"/> used to represent every trap in the system.
    /// </summary>
    [CreateAssetMenu(fileName = "TrapIconData", menuName = "UI Data/TrapIconData")]
    public class TrapIconData : ScriptableObject {
        [SerializeField] private List<TrapIcon> _icons;

        /// <summary>
        /// Finds the trap icon associated with the provided <see cref="Trap"/> data.
        /// </summary>
        /// <param name="trap"> The <see cref="Trap"/> class used to determine the trap type to find the related icon. </param>
        /// <returns> The trap icon sprite associated with the provided trap data. </returns>
        public Sprite TrapToImage(Trap trap) {
            switch (trap) {
                case SpikeTrap:
                    return this._icons.First(trapIcon => trapIcon.TrapName == "SpikeTrap").Icon;
                case SwingingAxeTrap:
                    return this._icons.First(trapIcon => trapIcon.TrapName == "SwingingAxeTrap").Icon;
                case IcicleTrap:
                    return this._icons.First(trapIcon => trapIcon.TrapName == "IcicleTrap").Icon;
                case AcidPitTrap:
                    return this._icons.First(trapIcon => trapIcon.TrapName == "AcidPitTrap").Icon;
                default:
                    return this._icons[0].Icon;
            }
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
        public string TrapName;
        public Sprite Icon;
    }
}
