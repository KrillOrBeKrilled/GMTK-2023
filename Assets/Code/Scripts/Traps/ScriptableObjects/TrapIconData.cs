using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KrillOrBeKrilled.Traps {
  [CreateAssetMenu(fileName = "TrapIconData", menuName = "UI Data/TrapIconData")]
  public class TrapIconData : ScriptableObject {
    [SerializeField] private List<TrapIcon> _icons;

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

  [Serializable]
  public class TrapIcon {
    public string TrapName;
    public Sprite Icon;
  }
}
