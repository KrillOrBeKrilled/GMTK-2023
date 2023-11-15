using KrillOrBeKrilled.Traps;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class TrapRequirementUI : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _amountText;
    [FormerlySerializedAs("_resourceTypeData")] [SerializeField] private ResourceIconData _resourceIconData;

    public void Hide() {
      this.gameObject.SetActive(false);
    }

    public void SetIconAmount(ResourceType type, int amount) {
      this.gameObject.SetActive(true);
      this._icon.sprite = this._resourceIconData.TypeToImage(type);
      this._amountText.SetText(amount.ToString());
    }
  }
}
