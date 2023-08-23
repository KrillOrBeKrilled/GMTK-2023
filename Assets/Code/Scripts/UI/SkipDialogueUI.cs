using Code.Scripts.Managers;
using Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

namespace Code.Scripts.UI {
  public class SkipDialogueUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private Image _completionImage;
    private float _skipStartTime = -1f;
    private float _skipHoldDuration = 1f;
    private UnityAction _onSkipComplete;

    public void OnPointerDown(PointerEventData eventData) {
      this._skipStartTime = Time.time;
      this._completionImage.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
      this.OnHoldStopped();
    }

    public void Initialize(UnityEvent onStartLevel, UnityAction onSkipComplete) {
      this._onSkipComplete = onSkipComplete;
      onStartLevel.AddListener(this.OnStartLevel);
    }

    private void Awake() {
      if (PlayerPrefsManager.ShouldSkipDialogue()) {
        this.gameObject.SetActive(false);
        return;
      }

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
        this._onSkipComplete?.Invoke();
        this._completionImage.fillAmount = 0f;
        this.OnHoldStopped();
        this.gameObject.SetActive(false);
      }
    }

    private void OnHoldStopped() {
      this._skipStartTime = -1f;
      this._completionImage.fillAmount = 0f;
      this._completionImage.gameObject.SetActive(false);
    }

    private void OnStartLevel() {
      this.gameObject.SetActive(false);
    }
  }
}
