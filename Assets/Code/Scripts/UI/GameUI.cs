using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
  [Header("Game UI References")]
  [SerializeField] private Image _foregroundImage;
  [SerializeField] private GameObject _pauseUI;
  [SerializeField] private EndgameUI _endgameUI;
  [SerializeField] private TMP_Text _coinsText;
  [SerializeField] private TrapSelectionBar _trapSelectionBar;

  [Header("Pause UI Events")]
  [SerializeField] private UnityEvent _onPaused;
  [SerializeField] private UnityEvent _onUnpaused;

  private const float FadeDuration = 0.5f;

  public void Initialize(GameManager gameManager, Player player) {
    gameManager.OnSetupComplete.AddListener(this.OnGameSetupComplete);
    gameManager.OnHenWon.AddListener(this.OnHenWon);
    gameManager.OnHenLost.AddListener(this.OnHenLost);

    this._trapSelectionBar.Initialize(player);
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
    CoinManager.Instance.OnCoinAmountChanged.AddListener(this.OnCoinsUpdated);
    PauseManager.Instance.OnPauseToggled.AddListener(this.OnPauseToggled);
  }

  private void OnCoinsUpdated(int amount) {
    this._coinsText.SetText($"{amount}");
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

    if (isPaused) {
      this._onPaused?.Invoke();
    } else {
      this._onUnpaused?.Invoke();
    }
  }

  private void OnHenWon(string message) {
    this._endgameUI.ShowHenWon(message);
  }

  private void OnHenLost(string message) {
    this._endgameUI.ShowHenLost(message);
  }
}
