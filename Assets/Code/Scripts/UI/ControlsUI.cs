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

    private void OnPlayerGrounded() {
      this._jumpButton.sprite = this._jumpImage;
    }

    private void OnPlayerFalling() {
      this._jumpButton.sprite = this._glideImage;
    }
  }
}
