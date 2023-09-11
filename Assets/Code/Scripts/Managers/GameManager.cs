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
    [SerializeField] private Hero _defaultHeroPrefab;
    [SerializeField] private Hero _druidHeroPrefab;

    [Header("Level")]
    [SerializeField] private RespawnPoint _respawnPointPrefab;
    [SerializeField] private EndgameTarget _endgameTargetPrefab;

    // --------------- Events ---------------
    public UnityEvent OnSetupComplete { get; private set; }
    public UnityEvent OnStartLevel { get; private set; }
    public UnityEvent<string> OnHenWon { get; private set; }
    public UnityEvent<string> OnHenLost { get; private set; }
    public UnityEvent<Hero> OnHeroSpawned { get; private set; }

    private Hero _heroActor;
    private readonly List<Hero> _heroes = new List<Hero>();

    private EndgameTarget _endgameTarget;
    private readonly List<RespawnPoint> _respawnPoints = new List<RespawnPoint>();
    private RespawnPoint _firstRespawnPoint;
    private RespawnPoint _activeRespawnPoint;

    private LevelData _levelData;
    private Queue<WaveData> _nextWavesDataQueue;
    private Queue<WaveData> _lastWavesDataQueue;
    private bool IsEndlessLevel => this._levelData != null && (this._levelData.Type == LevelData.LevelType.Endless);

    private const float EndlessLevelHealthIncreaseRate = 1.5f;

    private IEnumerator _waveSpawnCoroutine = null;

    public void LoadMainMenu() {
      PauseManager.Instance.UnpauseGame();
      PauseManager.Instance.SetIsPausable(false);
      this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadLevelsScene);
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
    }

#region YarnCommands

    [YarnCommand("spawn_hero_actor")]
    public void SpawnHeroActor() {
      this._heroActor = this.SpawnHero(HeroData.DefaultHero);

      if (this._heroActor.TryGetComponent(out YarnCharacter newYarnCharacter)) {
        YarnCharacterView.instance.RegisterYarnCharacter(newYarnCharacter);
        YarnCharacterView.instance.playerCharacter = newYarnCharacter;
      }
    }

    [YarnCommand("enter_hero_actor")]
    public void EnterHero() {
      this._heroActor.EnterLevel();
    }

    [YarnCommand("start_level")]
    public void StartLevel() {
      // Start recording the player input for the session
      this._playerManager.PlayerController.StartSession();

      if (this._heroActor != null) {
        this._heroActor.StartRunning();
      }

      CoinManager.Instance.StartCoinEarning();
      this.OnStartLevel?.Invoke();
    }

#endregion

    public void SkipDialogue() {
      this.StartCoroutine(this.SkipDialogueCoroutine());
    }

    private void Start() {
      // Create a copy to avoid modifying source
      LevelData sourceData = LevelManager.Instance.GetActiveLevelData();
      this._levelData = ScriptableObject.CreateInstance<LevelData>();
      this._levelData.DialogueName = sourceData.DialogueName;
      this._levelData.Type = sourceData.Type;
      this._levelData.RespawnPositions = sourceData.RespawnPositions.ToList();
      this._levelData.EndgameTargetPosition = sourceData.EndgameTargetPosition;
      this._levelData.WavesData = new WavesData() { WavesList = sourceData.WavesData.WavesList.ToList() };

      this._nextWavesDataQueue = new Queue<WaveData>(this._levelData.WavesData.WavesList);
      this._lastWavesDataQueue = new Queue<WaveData>(this._levelData.WavesData.WavesList);
      this._endgameTarget = Instantiate(this._endgameTargetPrefab, this._levelData.EndgameTargetPosition, Quaternion.identity, this.transform);

      foreach (Vector3 respawnPosition in this._levelData.RespawnPositions) {
        RespawnPoint newPoint = Instantiate(this._respawnPointPrefab, respawnPosition, Quaternion.identity, this.transform);
        this._respawnPoints.Add(newPoint);
      }

      this._activeRespawnPoint = this._respawnPoints.First();
      this._firstRespawnPoint = this._activeRespawnPoint;

      this._gameUI.Initialize(this, this._playerManager, this._firstRespawnPoint.transform, this._endgameTarget.transform);

      this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
      this._playerManager.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
      this._playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
      this._playerManager.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);

      this.OnSetupComplete?.Invoke();

      if (this._levelData.Type == LevelData.LevelType.Story) {
        this.StartStoryLevel();
      } else {
        this.StartEndlessLevel();
      }

      PauseManager.Instance.SetIsPausable(true);
    }

    private IEnumerator SkipDialogueCoroutine() {
      if (this._dialogueRunner.IsDialogueRunning) {
        this._dialogueRunner.Stop();
      }

      this._cameraShaker.StopShake();
      this._cameraSwitcher.ShowPlayer();

      // Skip a frame to ensure all scripts have initialized and called Start()
      yield return null;
      this.StartLevel();
      this._waveSpawnCoroutine = this.SpawnNextWave();
      this.StartCoroutine(this._waveSpawnCoroutine);
    }

    private void StartStoryLevel() {
      if (PlayerPrefsManager.ShouldSkipDialogue()) {
        this.SkipDialogue();
        return;
      }

      if (!this._dialogueRunner.yarnProject.NodeNames.Contains(this._levelData.DialogueName)) {
        Debug.LogError("Missing or Incorrect Dialogue Name, make sure provided dialogue name value is correct");
        return;
      }

      this._dialogueRunner.StartDialogue(this._levelData.DialogueName);
    }

    private void StartEndlessLevel() {
      this.SkipDialogue();
    }

    private void OnPlayerStateChanged(IPlayerState state, float xPos, float yPos, float zPos) {
      if (state is not GameOverState) return;

      this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");

      if (UGS_Analytics.Instance is null) return;
      UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
    }

    private void SelectedTrapIndexChanged(int trapIndex) {
      if (UGS_Analytics.Instance is null) {
        return;
      }

      bool isAffordable = this._playerManager.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;
      UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
    }

    private void OnTrapDeployed(int trapIndex) {
      if (UGS_Analytics.Instance is null) {
        return;
      }

      UGS_Analytics.DeployTrapCustomEvent(trapIndex);
    }


    private void OnHeroDied(Hero hero) {
      if (hero.TryGetComponent(out YarnCharacter diedCharacter)) {
        YarnCharacterView.instance.ForgetYarnCharacter(diedCharacter);
      }

      this._heroes.Remove(hero);

      bool noMoreWaves = !this.IsEndlessLevel && this._nextWavesDataQueue.Count <= 0;
      bool allHeroesDied = this._heroes.Count <= 0;
      if (noMoreWaves && allHeroesDied) {
        this.HenWon("All heroes were defeated. Good job!");
      } else if (allHeroesDied) {
        this._waveSpawnCoroutine = this.SpawnNextWave();
        this.StartCoroutine(this._waveSpawnCoroutine);
      }

      if (UGS_Analytics.Instance is null) return;
      Vector3 heroPos = hero.transform.position;
      UGS_Analytics.HeroDiedCustomEvent(heroPos.x, heroPos.y, heroPos.z);
    }

    private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
      if (UGS_Analytics.Instance is null) return;
      UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
    }

    private void HeroReachedLevelEnd() {
      this.HenLost("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
    }

    private void HenLost(string endgameMessage) {
      this._playerManager.PlayerController.DisablePlayerInput();

      if (this._waveSpawnCoroutine != null) {
        this.StopCoroutine(this._waveSpawnCoroutine);
      }

      this.StopAllHeroes();
      this.OnHenLost?.Invoke(endgameMessage);
    }

    private void HenWon(string message) {
      this._playerManager.PlayerController.DisablePlayerInput();
      FreezeCommand freezeCommand = new FreezeCommand(this._playerManager.PlayerController);
      this._playerManager.PlayerController.ExecuteCommand(freezeCommand);
      this.OnHenWon?.Invoke(message);
    }

    private void HenDied(string message) {
      Destroy(this._playerManager.gameObject);

      if (this._waveSpawnCoroutine != null) {
        this.StopCoroutine(this._waveSpawnCoroutine);
      }

      this._dialogueRunner.Stop();
      this.HenLost(message);
    }

    private IEnumerator SpawnNextWave() {
      if (this._nextWavesDataQueue.Count <= 0) {
        if (!this.IsEndlessLevel) {
          yield break;
        }

        this.GenerateNextWaves();
      }

      WaveData waveData = this._nextWavesDataQueue.Dequeue();
      foreach (HeroData heroData in waveData.Heroes) {
        this.SpawnHero(heroData);
        yield return new WaitForSeconds(waveData.HeroSpawnDelayInSeconds);
      }

      if (waveData.NextWaveSpawnDelayInSeconds < 0) {
        yield break;
      }

      yield return new WaitForSeconds(waveData.NextWaveSpawnDelayInSeconds);
      this._waveSpawnCoroutine = this.SpawnNextWave();
      yield return this._waveSpawnCoroutine;
    }

    private Hero SpawnHero(HeroData heroData) {
      Hero heroPrefab = this._defaultHeroPrefab;
      if (heroData.Type == HeroData.HeroType.Druid) {
        heroPrefab = this._druidHeroPrefab;
      }

      Hero newHero = Instantiate(heroPrefab, this._activeRespawnPoint.transform);
      newHero.Initialize(heroData);
      newHero.OnHeroDied.AddListener(this.OnHeroDied);
      newHero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

      this._heroes.Add(newHero);
      this.OnHeroSpawned.Invoke(newHero);
      return newHero;
    }

    private void GenerateNextWaves() {
      if (this._nextWavesDataQueue.Count > 0) {
        Debug.LogWarning("Attempted to generate next wave when current one is not empty.");
        return;
      }

      foreach (WaveData waveData in this._lastWavesDataQueue) {
        WaveData newWave = new WaveData() {
          Heroes = new List<HeroData>(),
          HeroSpawnDelayInSeconds = waveData.HeroSpawnDelayInSeconds,
          NextWaveSpawnDelayInSeconds = waveData.NextWaveSpawnDelayInSeconds
        };

        foreach (HeroData heroData in waveData.Heroes) {
          HeroData newHeroData = new HeroData() {
            Health = Mathf.FloorToInt(heroData.Health * EndlessLevelHealthIncreaseRate),
            Type = heroData.Type
          };

          newWave.Heroes.Add(newHeroData);
        }

        this._nextWavesDataQueue.Enqueue(newWave);
      }

      this._lastWavesDataQueue = new Queue<WaveData>(this._nextWavesDataQueue);
    }

    private void StopAllHeroes() {
      foreach (Hero hero in this._heroes) {
        hero.HeroMovement.ToggleMoving(false);
      }
    }
  }
}
