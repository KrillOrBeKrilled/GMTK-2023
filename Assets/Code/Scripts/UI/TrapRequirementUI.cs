using KrillOrBeKrilled.Traps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class TrapRequirementUI : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private ResourceTypeData _resourceTypeData;

    public void SetIconAmount(ResourceType type, int amount) {
      this._icon.sprite = this._resourceTypeData.TypeToImage(type);
      this._amountText.SetText(amount.ToString());
    }
  }
}
