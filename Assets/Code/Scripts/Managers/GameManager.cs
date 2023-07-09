using Input;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager> {
  [SerializeField] private GameUI _gameUI;
  [SerializeField] private Hero _heroPrefab;
  [SerializeField] private Player _player;
  [SerializeField] private EndgameTarget _endgameTarget;
  [SerializeField] private OutOfBoundsTrigger _outOfBoundsTrigger;

  public UnityEvent OnSetupComplete { get; private set; }
  public UnityEvent OnHenWon { get; private set; }
  public UnityEvent OnHenDied { get; private set; }
  public UnityEvent OnHeroReachedLevelEnd { get; private set; }

  private Hero _hero;

  public void LoadMainMenu() {
    PauseManager.Instance.UnpauseGame();
    PauseManager.Instance.SetIsPausable(false);
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadMainMenuScene);
  }

  public void LoadNextLevel() {
    PauseManager.Instance.UnpauseGame();
    print("Loading Next Level");
  }

  public void ReloadThisLevel() {
    PauseManager.Instance.UnpauseGame();
    PauseManager.Instance.SetIsPausable(false);
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.ReloadCurrentScene);
  }

  protected override void Awake() {
    base.Awake();
    this.OnSetupComplete = new UnityEvent();
    this.OnHenWon = new UnityEvent();
    this.OnHenDied = new UnityEvent();
    this.OnHeroReachedLevelEnd = new UnityEvent();
  }

  private void Start() {
    // Setup
    this._gameUI.Initialize(this);
    this._hero = FindObjectOfType<Hero>();

    this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
    this._player.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
    this._outOfBoundsTrigger.OnPlayerOutOfBounds.AddListener(this.HenDied);

    this.OnSetupComplete?.Invoke();
    PauseManager.Instance.SetIsPausable(true);
  }

  private void OnPlayerStateChanged(IPlayerState state) {
    if (state is GameOverState) {
      this.HenDied();
    }
  }

  private void GameWon() {
    this._player.PlayerController.DisablePlayerInput();
    this.OnHenWon?.Invoke();
  }

  private void HeroReachedLevelEnd() {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    this.OnHeroReachedLevelEnd?.Invoke();
  }

  private void HenDied() {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    Destroy(this._player.gameObject);
    this.OnHenDied?.Invoke();
  }
}
