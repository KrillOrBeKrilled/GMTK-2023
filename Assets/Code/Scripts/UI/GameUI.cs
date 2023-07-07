using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour{
  [SerializeField] private Image _foregroundImage;
  [SerializeField] private Image _sceneCover;
  [SerializeField] private GameObject _pauseUI;

  public void Initialize(GameManager gameManager) {
    gameManager.OnSetupComplete.AddListener(this.OnGameSetupComplete);
    gameManager.OnGameOver.AddListener(this.OnGameOver);
  }

  public void FadeInSceneCover(UnityAction onComplete) {
    this._sceneCover.gameObject.SetActive(true);
    this._sceneCover
      .DOFade(1, 0.5f)
      .OnComplete(() => onComplete?.Invoke());
  }

  private void Awake() {
    this._foregroundImage.gameObject.SetActive(true);
  }

  private void Start() {
    PauseManager.Instance.OnPauseToggled.AddListener(this.OnPauseToggled);
  }

  private void OnGameSetupComplete() {
    this._foregroundImage
      .DOFade(0, 1f)
      .OnComplete(() => this._foregroundImage.gameObject.SetActive(false));
  }

  private void OnPauseToggled(bool isPaused) {
    this._pauseUI.SetActive(isPaused);
  }

  private void OnGameOver() {

  }
}
