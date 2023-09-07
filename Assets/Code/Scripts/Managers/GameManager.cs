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
    [SerializeField] private LevelData _levelDataFile;
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

      // Create a copy to avoid modifying data source
      this._levelData = ScriptableObject.CreateInstance<LevelData>();
      this._levelData.Type = this._levelDataFile.Type;
      this._levelData.DialogueName = this._levelDataFile.DialogueName;
      this._levelData.EndgameTargetPosition = this._levelDataFile.EndgameTargetPosition;
      this._levelData.RespawnPositions = this._levelDataFile.RespawnPositions.ToList();
      this._levelData.WavesData = new WavesData() { WavesList = this._levelDataFile.WavesData.WavesList.ToList() };

      this._endgameTarget = Instantiate(this._endgameTargetPrefab, this._levelData.EndgameTargetPosition, Quaternion.identity, this.transform);

      foreach (Vector3 respawnPosition in this._levelData.RespawnPositions) {
        RespawnPoint newPoint = Instantiate(this._respawnPointPrefab, respawnPosition, Quaternion.identity, this.transform);
        this._respawnPoints.Add(newPoint);
      }

      this._activeRespawnPoint = this._respawnPoints.First();
      this._firstRespawnPoint = this._activeRespawnPoint;
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
      this._heroActor?.StartRunning();

      CoinManager.Instance.StartCoinEarning();
      this.OnStartLevel?.Invoke();
    }

#endregion

    public void SkipDialogue() {
      if (this._dialogueRunner.IsDialogueRunning) {
        this._dialogueRunner.Stop();
      }

      this._cameraShaker.StopShake();
      this._cameraSwitcher.ShowPlayer();
      this.StartLevel();
      this.StartCoroutine(this.SpawnNextWave());
    }

    private void Start() {
      this._gameUI.Initialize(this, this._playerManager);

      this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
      this._playerManager.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
      this._playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
      this._playerManager.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);

      this._playerManager.Initialize(this._firstRespawnPoint.transform, this._endgameTarget.transform);

      this.OnSetupComplete?.Invoke();

      if (this._levelData.Type == LevelData.LevelType.Story) {
        this.StartStoryLevel();
      } else {
        this.StartEndlessLevel();
      }

      PauseManager.Instance.SetIsPausable(true);
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

      bool noMoreWaves = this._levelData.WavesData.WavesList.Count <= 0;
      bool allHeroesDied = this._heroes.Count <= 0;
      if (noMoreWaves && allHeroesDied) {
        this.OnHenWon?.Invoke("All heroes were defeated. Good job!");
      } else if (allHeroesDied) {
        this.StartCoroutine(this.SpawnNextWave());
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
      this._playerManager.PlayerController.DisablePlayerInput();
      this.StopAllHeroes();
      this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
    }

    private void HenDied(string message) {
      this._playerManager.PlayerController.DisablePlayerInput();
      this.StopAllHeroes();

      Destroy(this._playerManager.gameObject);
      this.OnHenLost?.Invoke(message);
    }

    private IEnumerator SpawnNextWave() {
      if (this._levelData.WavesData.WavesList.Count <= 0) {
        Debug.LogWarning("Trying to spawn next wave when there are no more waves to spawn");
        yield break;
      }

      WaveData waveData = this._levelData.WavesData.WavesList[0];
      for (int i = 0; i < waveData.Heroes.Count; i++) {
        this.SpawnHero(waveData.Heroes[i]);
        yield return new WaitForSeconds(waveData.HeroSpawnDelayInSeconds);
      }

      this._levelData.WavesData.WavesList.RemoveAt(0);

      if (waveData.NextWaveSpawnDelayInSeconds < 0) {
        yield break;
      }

      yield return new WaitForSeconds(waveData.NextWaveSpawnDelayInSeconds);
      yield return this.SpawnNextWave();
    }

    private Hero SpawnHero(HeroData heroData) {
      Hero newHero = Instantiate(this._heroPrefab, this._activeRespawnPoint.transform);
      newHero.Initialize(heroData, this._firstRespawnPoint.transform, this._endgameTarget.transform);
      newHero.OnHeroDied.AddListener(this.OnHeroDied);
      newHero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

      this._heroes.Add(newHero);
      this.OnHeroSpawned.Invoke(newHero);
      return newHero;
    }

    private void StopAllHeroes() {
      foreach (Hero hero in this._heroes) {
        hero.HeroMovement.ToggleMoving(false);
      }
    }
  }
}
