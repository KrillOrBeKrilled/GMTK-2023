using Dialogue;
using Heroes;
using Model;
using Player;
using System.Collections;
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

    [Header("Dialogue References")]
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private CameraSwitcher _cameraSwitcher;
    [SerializeField] private CameraShaker _cameraShaker;

    [Header("Heroes")]
    [SerializeField] private Hero _heroPrefab;

    [Header("Level")]
    [SerializeField] private List<RespawnPoint> _respawnPoints;
    [SerializeField] private RespawnPoint _firstRespawnPoint;
    [SerializeField] private EndgameTarget _endgameTarget;
    private RespawnPoint _activeRespawnPoint;

    // --------------- Events ---------------
    public UnityEvent OnSetupComplete { get; private set; }
    public UnityEvent OnStartLevel { get; private set; }
    public UnityEvent<string> OnHenWon { get; private set; }
    public UnityEvent<string> OnHenLost { get; private set; }
    public UnityEvent<Hero> OnHeroSpawned { get; private set; }

    // private Hero _hero;
    private static HeroData _defaultHero = new HeroData() { Health = 100, Type = HeroData.HeroType.Default };

    private static WaveData _wave1 = new WaveData() { Heroes = new List<HeroData>() {_defaultHero, _defaultHero}, HeroSpawnDelayInSeconds = 1f, NextWaveSpawnDelayInSeconds = 10f};
    private static WaveData _wave2 = new WaveData() { Heroes = new List<HeroData>() {_defaultHero, _defaultHero, _defaultHero, _defaultHero, _defaultHero}, HeroSpawnDelayInSeconds = 1.5f, NextWaveSpawnDelayInSeconds = 15f};
    private static WaveData _wave3 = new WaveData() { Heroes = new List<HeroData>() {_defaultHero, _defaultHero, _defaultHero, _defaultHero, _defaultHero}, HeroSpawnDelayInSeconds = 3f, NextWaveSpawnDelayInSeconds = -1f};
    private static List<WaveData> _waves = new List<WaveData>() { _wave1, _wave2, _wave3};

    private List<Hero> _heroes = new List<Hero>();
    private WavesData _wavesData = new WavesData() { WavesList = _waves };

    // TODO : Fix dialogue

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
      this.OnHeroSpawned = new UnityEvent<Hero>();

      this._activeRespawnPoint = this._respawnPoints.First();
    }

    [YarnCommand("enter_hero")]
    public void EnterHero() {
      // this._hero.EnterLevel();
    }

    [YarnCommand("start_level")]
    public void StartLevel() {
      // For playtesting analytics, start recording the player input for the session
      this._playerManager.PlayerController.StartSession();

      // this._hero.StartRunning();
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
      this._playerManager.Initialize(this._firstRespawnPoint.transform, this._endgameTarget.transform);

      this.StartCoroutine(this.SpawnNextWave());
      // this._hero.HeroMovement.ToggleMoving(false);

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
      if (hero.TryGetComponent(out YarnCharacter diedCharacter)) {
        YarnCharacterView.instance.ForgetYarnCharacter(diedCharacter);
      }

      this._heroes.Remove(hero);

      bool noMoreWaves = this._wavesData.WavesList.Count <= 0;
      bool allHeroesDied = this._heroes.Count <= 0;
      if (noMoreWaves && allHeroesDied) {
        // TODO: better text line later?
        this.OnHenWon?.Invoke("All heroes were defeated. Good job!");
      } else if (allHeroesDied) {
        this.StartCoroutine(this.SpawnNextWave());
      }

      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;
      Vector3 heroPos = hero.transform.position;
      UGS_Analytics.HeroDiedCustomEvent(heroPos.x, heroPos.y, heroPos.z);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
      // Send Analytics data
      if (UGS_Analytics.Instance is null) return;

      UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
    }

    private void HeroReachedLevelEnd() {
      this._playerManager.PlayerController.DisablePlayerInput();
      // this._hero.HeroMovement.ToggleMoving(false);
      this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
    }

    private void HenDied(string message) {
      this._playerManager.PlayerController.DisablePlayerInput();
      // this._hero.HeroMovement.ToggleMoving(false);

      Destroy(this._playerManager.gameObject);
      this.OnHenLost?.Invoke(message);
    }

    private IEnumerator SpawnNextWave() {
      WaveData waveData = this._wavesData.WavesList[0];
      for (int i = 0; i < waveData.Heroes.Count; i++) {
        this.SpawnHero();
        yield return new WaitForSeconds(waveData.HeroSpawnDelayInSeconds);
      }

      this._wavesData.WavesList.RemoveAt(0);

      if (waveData.NextWaveSpawnDelayInSeconds < 0) {
        yield break;
      }

      yield return new WaitForSeconds(waveData.NextWaveSpawnDelayInSeconds);
      yield return this.SpawnNextWave();
    }

    private void SpawnHero() {
      Hero newHero = Instantiate(this._heroPrefab, this._activeRespawnPoint.transform);
      newHero.Initialize(this._firstRespawnPoint.transform, this._endgameTarget.transform);
      newHero.OnHeroDied.AddListener(this.OnHeroDied);
      newHero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

      if (newHero.TryGetComponent(out YarnCharacter newYarnCharacter)) {
        YarnCharacterView.instance.RegisterYarnCharacter(newYarnCharacter);
        YarnCharacterView.instance.playerCharacter = newYarnCharacter;
      }

      this._heroes.Add(newHero);
      this.OnHeroSpawned.Invoke(newHero);
    }
  }
}
