using KrillOrBeKrilled.Traps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class ResourceAmountUI : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private ResourceIconData _resourceIconData;
    [SerializeField] private ResourceType _resourceType;

    public ResourceType ResourceType => _resourceType;

    public void Hide() {
      this.gameObject.SetActive(false);
    }

    public void SetIconAmount(ResourceType type, int amount) {
      this.gameObject.SetActive(true);
      this._icon.sprite = this._resourceIconData.TypeToImage(type);
      this._amountText.SetText(amount.ToString());
      this._resourceType = type;
    }
  }
}
