using Dialogue;
using Heroes;
using Player;
using System.Collections.Generic;
using System.Linq;
using UGSAnalytics;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Managers {
  public class GameManager : Singleton<GameManager> {
    [Header("References")]
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private EndgameTarget _endgameTarget;

    [Header("Dialogue References")]
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private CameraSwitcher _cameraSwitcher;
    [SerializeField] private CameraShaker _cameraShaker;

    [Header("Heroes")]
    [SerializeField] private Hero _heroPrefab;

    [Header("Level")]
    [SerializeField] private List<RespawnPoint> _respawnPoints;
    private RespawnPoint _activeRespawnPoint;

    public UnityEvent OnSetupComplete { get; private set; }
    public UnityEvent OnStartLevel { get; private set; }
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

      this._activeRespawnPoint = this._respawnPoints.First();
      this.SpawnHero();
    }

    [YarnCommand("enter_hero")]
    public void EnterHero() {
      this._hero.EnterLevel();
    }

    [YarnCommand("start_level")]
    public void StartLevel() {
      // For playtesting analytics, start recording the player input for the session
      this._playerManager.PlayerController.StartSession();

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
      this._gameUI.Initialize(this, this._playerManager);

      this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
      this._playerManager.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
      this._playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
      this._playerManager.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);

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
      if (state is not GameOverState) return;

      this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");

      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
    }

    private void SelectedTrapIndexChanged(int trapIndex) {
      var isAffordable = this._playerManager.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;

      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
    }

    private void OnTrapDeployed(int trapIndex) {
      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      UGS_Analytics.DeployTrapCustomEvent(trapIndex);
    }


    private void OnHeroDied(Hero hero) {
      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      Vector3 heroPos = hero.transform.position;
      UGS_Analytics.HeroDiedCustomEvent(heroPos.x, heroPos.y, heroPos.z);

      if (!hero.TryGetComponent(out YarnCharacter diedCharacter)) {
        return;
      }

      YarnCharacterView.instance.ForgetYarnCharacter(diedCharacter);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
    }

    private void HeroReachedLevelEnd() {
      this._playerManager.PlayerController.DisablePlayerInput();
      this._hero.HeroMovement.ToggleMoving(false);
      this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
    }

    private void HenDied(string message) {
      this._playerManager.PlayerController.DisablePlayerInput();
      this._hero.HeroMovement.ToggleMoving(false);

      Destroy(this._playerManager.gameObject);
      this.OnHenLost?.Invoke(message);
    }

    private void SpawnHero() {
      this._hero = Instantiate(this._heroPrefab, this._activeRespawnPoint.transform);
      this._hero.OnHeroDied.AddListener(this.OnHeroDied);
      this._hero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);
      this._hero.HeroMovement.ToggleMoving(false);

      if (!this._hero.TryGetComponent(out YarnCharacter newYarnCharacter)) {
        return;
      }

      YarnCharacterView.instance.RegisterYarnCharacter(newYarnCharacter);
      YarnCharacterView.instance.playerCharacter = newYarnCharacter;
    }
  }
}
