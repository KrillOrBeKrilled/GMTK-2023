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
  public UnityEvent<string> OnHenWon { get; private set; }
  public UnityEvent<string> OnHenLost { get; private set; }

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
    this.OnHenWon = new UnityEvent<string>();
    this.OnHenLost = new UnityEvent<string>();
  }

  private void Start() {
    // Setup
    this._gameUI.Initialize(this);
    this._hero = FindObjectOfType<Hero>();

    this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
    this._player.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
    this._outOfBoundsTrigger.OnPlayerOutOfBounds.AddListener(this.HenOutOfBounds);

    this.OnSetupComplete?.Invoke();
    PauseManager.Instance.SetIsPausable(true);
  }

  private void OnPlayerStateChanged(IPlayerState state) {
    if (state is GameOverState) {
      this.HenDied("Hero managed to take you down hen.\nDon't you dream about that promotion I mentioned last time!");
    }
  }

  private void GameWon() {
    this._player.PlayerController.DisablePlayerInput();
    this.OnHenWon?.Invoke("Hero was stopped, good work hen!");
  }

  private void HeroReachedLevelEnd() {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    this.OnHenLost?.Invoke("Hero managed to reach his goal and do heroic things.\nHen, you failed me!");
  }

  private void HenOutOfBounds() {
    this.HenDied("What are you doing here?\nI told you, you should always keep an eye on the hero!");
  }

  private void HenDied(string message) {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    Destroy(this._player.gameObject);
    this.OnHenLost?.Invoke(message);
  }
}
