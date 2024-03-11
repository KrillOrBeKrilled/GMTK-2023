using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// CooldownUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// TODO: Fill this out
    /// </summary>
    public class CooldownUI : MonoBehaviour {
        public Image OverlayImage;

        public void OnCooldownProgressUpdated(float percentage) {
            this.OverlayImage.fillAmount = percentage;
        }
    }
}
