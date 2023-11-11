using KrillOrBeKrilled.Traps;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class TrapRequirementsUI : MonoBehaviour {
    [SerializeField] private List<TrapRequirementUI> _trapRequirements;

    private void Start() {
      this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 5);
      this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 6);
      this._trapRequirements[2].SetIconAmount(ResourceType.Slime, 7);
    }
  }
}
