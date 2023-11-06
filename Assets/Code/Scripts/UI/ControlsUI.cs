using KrillOrBeKrilled.Core.Player;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class ControlsUI : MonoBehaviour {
    [SerializeField] private UIButton _jumpButton;
    [SerializeField] private Sprite _jumpImage;
    [SerializeField] private Sprite _jumpHighlightedImage;
    [SerializeField] private Sprite _glideImage;
    [SerializeField] private Sprite _glideHighlightedImage;

    public void Initialize(PlayerController playerController) {
      playerController.OnPlayerGrounded.AddListener(this.OnPlayerGrounded);
      playerController.OnPlayerFalling.AddListener(this.OnPlayerFalling);
    }

    /// <summary> Sets jump button's sprite to a jump icon. </summary>
    private void OnPlayerGrounded() {
      this._jumpButton.SetButtonSprites(this._jumpImage, this._jumpHighlightedImage);
    }

    /// <summary> Sets jump button's sprite to a glide icon. </summary>
    private void OnPlayerFalling() {
      this._jumpButton.SetButtonSprites(this._glideImage, this._glideHighlightedImage);
    }
  }
}
