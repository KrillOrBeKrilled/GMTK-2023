using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

namespace Code.Scripts.UI {
  public class SkipDialogueUI : MonoBehaviour {
    [SerializeField] private Image _completionImage;
    private float _skipStartTime = -1f;
    private float _skipHoldDuration = -1f;

    public void Initialize(PlayerController playerController) {
      playerController.OnSkipDialogueStarted.AddListener(this.OnSkipStarted);
      playerController.OnSkipDialogueCancelled.AddListener(this.OnSkipCancelled);
      playerController.OnSkipDialoguePerformed.AddListener(this.OnSkipPerformed);

      this.OnHoldStopped();
    }

    private void Update() {
      if (this._skipStartTime < 0f) {
        return;
      }

      float timeElapsed = Time.time - this._skipStartTime;
      float completionPercentage = timeElapsed / this._skipHoldDuration;
      this._completionImage.fillAmount = completionPercentage;

      if (completionPercentage >= 0.99f) {
        this._completionImage.fillAmount = 0f;
      }
    }

    private void OnSkipStarted(InputAction.CallbackContext ctx) {
      if (ctx.interaction is not HoldInteraction holdInteraction) {
        return;
      }

      this._skipStartTime = Time.time;
      this._skipHoldDuration = holdInteraction.duration;
      this._completionImage.gameObject.SetActive(true);
    }

    private void OnSkipCancelled(InputAction.CallbackContext ctx) {
      this.OnHoldStopped();
    }

    private void OnSkipPerformed(InputAction.CallbackContext ctx) {
      this.OnHoldStopped();
      this.gameObject.SetActive(false);
    }

    private void OnHoldStopped() {
      this._skipStartTime = -1f;
      this._skipHoldDuration = -1f;

      this._completionImage.fillAmount = 0f;
      this._completionImage.gameObject.SetActive(false);
    }
  }
}
