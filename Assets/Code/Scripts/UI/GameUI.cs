using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour{
  [SerializeField] private Image _foregroundImage;
  [SerializeField] private GameObject _pauseUI;
  [SerializeField] private EndgameUI _endgameUI;

  private const float FadeDuration = 0.5f;

  public void Initialize(GameManager gameManager) {
    gameManager.OnSetupComplete.AddListener(this.OnGameSetupComplete);

    gameManager.OnHenWon.AddListener(this.OnHenWon);
    gameManager.OnHenLost.AddListener(this.OnHenLost);
  }

  public void FadeInSceneCover(UnityAction onComplete) {
    this._foregroundImage.gameObject.SetActive(true);
    this._foregroundImage
      .DOFade(1, FadeDuration)
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
      .DOFade(0, FadeDuration)
      .OnComplete(() => {
        this._foregroundImage.gameObject.SetActive(false);
      });
  }

  private void OnPauseToggled(bool isPaused) {
    this._pauseUI.SetActive(isPaused);
  }

  private void OnHenWon(string message) {
    this._endgameUI.ShowHenWon(message);
  }

  private void OnHenLost(string message) {
    this._endgameUI.ShowHenLost(message);
  }
}
