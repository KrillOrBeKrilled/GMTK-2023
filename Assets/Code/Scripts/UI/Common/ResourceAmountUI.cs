using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Traps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// ResourceAmountUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages a singular resource display, exposing methods to update the resource Type
    /// icon and quantity.
    /// </summary>
    public class ResourceAmountUI : MonoBehaviour {
        [Tooltip("Displays the icon of the associated resource Type.")] 
        [SerializeField] private Image _icon;
        [Tooltip("Displays the quantity held of the associated resource Type.")] 
        [SerializeField] private TMP_Text _amountText;
        [Tooltip("The cumulative resource icon data used to find the icon to display for the associated resource Type.")] 
        [SerializeField] private ResourceIconData _resourceIconData;
        [SerializeField] private ResourceType _resourceType;

        public ResourceType ResourceType => _resourceType;
            
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Disables the resource UI display GameObject.
        /// </summary>
        public void Hide() {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the icon and quantity display.
        /// </summary>
        /// <param name="type"> The resource Type used to find the associated icon to display. </param>
        /// <param name="amount"> The quantity associated with the resource Type to display. </param>
        public void SetIconAmount(ResourceType type, int amount) {
            this.gameObject.SetActive(true);
            this._icon.sprite = this._resourceIconData.TypeToImage(type);
            this._amountText.SetText(amount.ToString());
            this._resourceType = type;
        }
        
        #endregion
    }
}
