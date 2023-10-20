using KrillOrBeKrilled.Core.Player;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class ControlsUI : MonoBehaviour {
    [SerializeField] private Image _jumpButton;
    [SerializeField] private Sprite _jumpImage;
    [SerializeField] private Sprite _glideImage;

    public void Initialize(PlayerController playerController) {
      playerController.OnPlayerGrounded.AddListener(this.OnPlayerGrounded);
      playerController.OnPlayerFalling.AddListener(this.OnPlayerFalling);
    }

    /// <summary> Sets jump button's sprite to a jump icon. </summary>
    private void OnPlayerGrounded() {
      this._jumpButton.sprite = this._jumpImage;
    }

    /// <summary> Sets jump button's sprite to a glide icon. </summary>
    private void OnPlayerFalling() {
      this._jumpButton.sprite = this._glideImage;
    }
  }
}
