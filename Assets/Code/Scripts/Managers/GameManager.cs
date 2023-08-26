using Code.Scripts.UI;
using Input;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Code.Scripts.Managers {
  public class GameManager : Singleton<GameManager> {
    [Header("References")]
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private Player _player;
    [SerializeField] private EndgameTarget _endgameTarget;

    [Header("Dialogue References")]
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private CameraSwitcher _cameraSwitcher;
    [SerializeField] private CameraShaker _cameraShaker;

    [Header("Heroes")]
    [SerializeField] private Hero _hero;

    public UnityEvent OnSetupComplete { get; private set; }
    public UnityEvent OnStartLevel { get; private set; }
    public UnityEvent<string> OnHenWon { get; private set; }
    public UnityEvent<string> OnHenLost { get; private set; }

    public void LoadMainMenu() {
      PauseManager.Instance.UnpauseGame();
      PauseManager.Instance.SetIsPausable(false);
      this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadMainMenuScene);
    }

    public void LoadNextLevel() {
      PauseManager.Instance.UnpauseGame();
    }

    public void ReloadThisLevel() {
      PauseManager.Instance.UnpauseGame();
      PauseManager.Instance.SetIsPausable(false);
      this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.ReloadCurrentScene);
    }

    protected override void Awake() {
      base.Awake();
      this.OnSetupComplete = new UnityEvent();
      this.OnStartLevel = new UnityEvent();
      this.OnHenWon = new UnityEvent<string>();
      this.OnHenLost = new UnityEvent<string>();
    }

    [YarnCommand("enter_hero")]
    public void EnterHero()
    {
      this._hero.EnterLevel();
    }

    [YarnCommand("start_level")]
    public void StartLevel()
    {
      this._hero.StartRunning();
      CoinManager.Instance.StartCoinEarning();
      this.OnStartLevel?.Invoke();
    }

    public void SkipDialogue() {
      if (this._dialogueRunner.IsDialogueRunning) {
        this._dialogueRunner.Stop();
      }

      this._cameraShaker.StopShake();
      this._cameraSwitcher.ShowPlayer();
      this.StartLevel();
    }

    private void Start() {
      // Setup
      this._gameUI.Initialize(this, this._player);

      this._hero.ResetHero();

      this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
      this._player.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
      this._player.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
      this._player.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);
      this._hero.OnGameOver.AddListener(this.GameWon);
      this._hero.OnHeroDied.AddListener(this.OnHeroDied);
      this._hero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

      this.OnSetupComplete?.Invoke();

      if (PlayerPrefsManager.ShouldSkipDialogue()) {
        this.SkipDialogue();
      } else {
        this._cameraSwitcher.ShowStart();
        this._cameraShaker.StopShake();
        this._dialogueRunner.StartDialogue(this._dialogueRunner.startNode);
      }

      PauseManager.Instance.SetIsPausable(true);
    }

    private void OnPlayerStateChanged(IPlayerState state, float xPos, float yPos, float zPos) {
      if (state is GameOverState) {
        // Send Analytics data before ending the game
        UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);

        this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");
      }
    }

    private void SelectedTrapIndexChanged(int trapIndex)
    {
      var isAffordable = this._player.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;

      // Send Analytics data
      UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
    }

    private void OnTrapDeployed(int trapIndex) {
      // Send Analytics data
      UGS_Analytics.DeployTrapCustomEvent(trapIndex);
    }


    private void OnHeroDied(int numberLives, float xPos, float yPos, float zPos)
    {
      // Send Analytics data before ending the game
      UGS_Analytics.HeroDiedCustomEvent(numberLives, xPos, yPos, zPos);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos)
    {
      // Send Analytics data
      UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
    }

    private void GameWon() {
      this._player.PlayerController.DisablePlayerInput();
      this.OnHenWon?.Invoke("The Hero was stopped, good work Hendall!");
    }

    private void HeroReachedLevelEnd() {
      this._player.PlayerController.DisablePlayerInput();
      this._hero.HeroMovement.ToggleMoving(false);
      this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
    }

    private void HenDied(string message) {
      this._player.PlayerController.DisablePlayerInput();
      this._hero.HeroMovement.ToggleMoving(false);

      Destroy(this._player.gameObject);
      this.OnHenLost?.Invoke(message);
    }
  }
}
