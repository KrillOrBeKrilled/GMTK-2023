using KrillOrBeKrilled.Player;
using UnityEngine;

//*******************************************************************************************
// ControlsUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages the updates to the control buttons' visual appearance in
    /// correspondence to the player's grounded and falling state.
    /// </summary>
    public class ControlsUI : MonoBehaviour {
        [Tooltip("The button controller associated with the on-screen jump button.")]
        [SerializeField] private UIButton _attackButton;
        [Tooltip("The button controller associated with the on-screen jump button.")]
        [SerializeField] private UIButtonTarget _jumpButtonTarget;
        [Tooltip("The jump image shown to indicate the jump button is not pressed.")]
        [SerializeField] private Sprite _jumpImage;
        [Tooltip("The jump image shown to indicate the jump button is pressed.")]
        [SerializeField] private Sprite _jumpHighlightedImage;
        [Tooltip("The glide image shown to indicate the jump button will enact a glide state and is not pressed.")]
        [SerializeField] private Sprite _glideImage;
        [Tooltip("The glide image shown to indicate the jump button will enact a glide state and is pressed.")]
        [SerializeField] private Sprite _glideHighlightedImage;

        [Header("Control Cooldown Overlays")] 
        [SerializeField] private CooldownUI _attackCooldown;

        //========================================
        // Public Methods
        //========================================
            
        #region Public Methods
        
        /// <summary>
        /// Sets up all listeners to operate the controls UI.
        /// </summary>
        /// <param name="player"> The <see cref="PlayerCharacter"/> to track for changes to the player's
        /// grounded and falling states. </param>
        public void Initialize(PlayerCharacter player) {
            player.OnPlayerGrounded.AddListener(this.OnPlayerGrounded);
            player.OnPlayerFalling.AddListener(this.OnPlayerFalling);
            
            player.OnAttackCooldownUpdated.AddListener(this._attackCooldown.OnCooldownProgressUpdated);
            player.OnAttackEnabledUpdated.AddListener(this.OnAttackCooldownUpdated);
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
            
        #region Private Methods
        
        /// <summary>
        /// Sets jump button's sprite to a glide icon.
        /// </summary>
        private void OnPlayerFalling() {
            this._jumpButtonTarget.SetButtonSprites(this._glideImage, this._glideHighlightedImage);
        }

        /// <summary>
        /// Sets jump button's sprite to a jump icon.
        /// </summary>
        private void OnPlayerGrounded() {
            this._jumpButtonTarget.SetButtonSprites(this._jumpImage, this._jumpHighlightedImage);
        }

        private void OnAttackCooldownUpdated(bool isEnabled) {
            this._attackButton.SetInteractable(isEnabled);
        }
        
        #endregion
    }
}
