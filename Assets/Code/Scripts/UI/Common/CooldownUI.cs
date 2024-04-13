using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// CooldownUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Updates an overlay image fill to visualize a cooldown timer on UI elements.
    /// </summary>
    /// <remarks> Methods to update the cooldown fill are exposed to be subscribed to
    /// events externally for greater flexibility and reuse. </remarks>
    public class CooldownUI : MonoBehaviour {
        public Image OverlayImage;

        /// <summary>
        /// Updates the cooldown fill corresponding to the specified percentage.
        /// </summary>
        /// <param name="percentage"> The fill amount to set for the cooldown overlay.  </param>
        public void OnCooldownProgressUpdated(float percentage) {
            this.OverlayImage.fillAmount = percentage;
        }
    }
}
