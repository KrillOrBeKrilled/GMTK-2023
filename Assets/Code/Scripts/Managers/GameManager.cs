using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager> {
  [SerializeField] private GameUI _gameUI;
  [SerializeField] private Hero _heroPrefab;
  [SerializeField] private Player _player;

  public UnityEvent OnSetupComplete { get; private set; }
  public UnityEvent OnGameWon { get; private set; }
  public UnityEvent OnGameLost { get; private set; }

  private Hero _hero;

  public void LoadMainMenu() {
    PauseManager.Instance.UnpauseGame();
    PauseManager.Instance.SetIsPausable(false);
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadMainMenuScene);
  }

  public void ReloadGameScene() {
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadGameScene);
  }

  protected override void Awake() {
    base.Awake();
    this.OnSetupComplete = new UnityEvent();
    this.OnGameWon = new UnityEvent();

    this.OnGameLost = new UnityEvent();
    this.OnGameLost.AddListener(this.GameOver);
  }

  private void Start() {
    // Setup
    this._gameUI.Initialize(this);
    PauseManager.Instance.SetIsPausable(true);
    this._hero = FindObjectOfType<Hero>();

    this.OnSetupComplete?.Invoke();
  }

  private void GameOver() {
    this._hero.HeroMovement.ToggleMoving(false);
  }
}
