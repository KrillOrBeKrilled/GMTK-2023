using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Cameras;
using KrillOrBeKrilled.Core.Player;
using KrillOrBeKrilled.Dialogue;
using KrillOrBeKrilled.Environment;
using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.UGSAnalytics;
using KrillOrBeKrilled.UI;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

//*******************************************************************************************
// GameManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Core {
    /// <summary>
    /// Acts as a central control panel for the main workings of a level and the actors
    /// within, including cutscene sequences and the handling of events through major
    /// state changes of the <see cref="Hero"/> and <see cref="PlayerController"/>.
    /// </summary>
    /// <remarks> Acts as an intermediary between <see cref="UGS_Analytics"/> and the
    /// other actors of the level to collect and send data. </remarks>
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

        // ------------- Sound Effects ---------------
        private HeroSoundsController _heroSoundsController;

        // ----------------- Events ------------------
        [Tooltip("Tracks when hero and UI data are set up and this manager sets up other critical gameplay system " +
                 "references.")]
        public UnityEvent OnSetupComplete { get; private set; }
        [Tooltip("Tracks when the main gameplay loop begins.")]
        public UnityEvent OnStartLevel { get; private set; }
        [Tooltip("Tracks when the player beats the game.")]
        public UnityEvent<string> OnHenWon { get; private set; }
        [Tooltip("Tracks when the player loses the game.")]
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
        
        protected override void Awake() {
            base.Awake();
            TryGetComponent(out this._heroSoundsController);
            
            this.OnSetupComplete = new UnityEvent();
            this.OnStartLevel = new UnityEvent();
            this.OnHenWon = new UnityEvent<string>();
            this.OnHenLost = new UnityEvent<string>();
            this.OnHeroSpawned = new UnityEvent<Hero>();
        }
        
        /// <remarks> Invokes the <see cref="OnSetupComplete"/> event. </remarks>
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
            
            this._gameUI.Initialize(OnSetupComplete, OnHenWon, OnHenLost, OnHeroSpawned, 
                this._playerManager.PlayerController.OnSelectedTrapIndexChanged, 
                this._playerManager.TrapController.Traps, OnStartLevel, SkipDialogue, this._playerManager.transform,
                this._firstRespawnPoint.transform, this._endgameTarget.transform);

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

        //========================================
        // Scene Management
        //========================================
        
        /// <summary> Disables pause controls and loads the MainMenu scene. </summary>
        public void LoadMainMenu() {
            PauseManager.Instance.UnpauseGame();
            PauseManager.Instance.SetIsPausable(false);
            this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadLevelsScene);
        }

        /// <summary> Unpauses the game. </summary>
        public void LoadNextLevel() {
            PauseManager.Instance.UnpauseGame();
        }

        /// <summary> Disables pause controls and reloads the current level. </summary>
        public void ReloadThisLevel() {
            PauseManager.Instance.UnpauseGame();
            PauseManager.Instance.SetIsPausable(false);
            this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.ReloadCurrentScene);
        }
        
        //========================================
        // Level Sequence
        //========================================
        
#region YarnCommands
        [YarnCommand("spawn_hero_actor")]
        public void SpawnHeroActor() {
            this._heroActor = this.SpawnHero(HeroData.DefaultHero);

            if (this._heroActor.TryGetComponent(out YarnCharacter newYarnCharacter)) {
                YarnCharacterView.instance.RegisterYarnCharacter(newYarnCharacter);
                YarnCharacterView.instance.playerCharacter = newYarnCharacter;
            }
        }
        
        /// <summary> Triggers the sequence to make the hero enter the level. </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("enter_hero_actor")]
        public void EnterHero() {
            this._heroActor.EnterLevel();
        }

        /// <summary> Enables player controls and recording features, hero movement, and timed coin earning. </summary>
        /// <remarks> Invokes the <see cref="OnStartLevel"/> event. Can be accessed as a YarnCommand.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, begins the
        /// recorded input playback feature. Otherwise, begins recording the player input. </p></remarks>
        [YarnCommand("start_level")]
        public void StartLevel() {
            this._playerManager.PlayerController.StartSession();

            if (this._heroActor != null) {
                this._heroActor.StartRunning();
            }

            CoinManager.Instance.StartCoinEarning();
            this.OnStartLevel?.Invoke();
        }
#endregion

        /// <summary>
        /// Aborts the dialogue player if the dialogue is actively running and immediately focuses the camera
        /// on the player before beginning the level gameplay.
        /// </summary>
        public void SkipDialogue() {
            this.StartCoroutine(this.SkipDialogueCoroutine());
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
            newHero.Initialize(heroData, _heroSoundsController);
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

        //========================================
        // Event Listeners
        //========================================

        /// <summary>
        /// Ends the game and records analytics player death data if the player state is <see cref="GameOverState"/>. 
        /// </summary>
        /// <param name="state"> The <see cref="PlayerController"/> state. </param>
        /// <param name="xPos"> The player's current position along the x-axis. </param>
        /// <param name="yPos"> The player's current position along the y-axis. </param>
        /// <param name="zPos"> The player's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnPlayerStateChanged"/> event. </remarks>
        private void OnPlayerStateChanged(IPlayerState state, float xPos, float yPos, float zPos) {
            if (state is not GameOverState) {
                return;
            }

            this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");

            if (UGS_Analytics.Instance is null) {
                return;
            }
            UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
        }
        
        /// <summary>
        /// Disables the player input and hero movement, destroying the player GameObject, and triggers the loss UI.
        /// </summary>
        /// <param name="message"> The message to be displayed on the loss UI. </param>
        /// <remarks> Helper for <see cref="GameManager.OnPlayerStateChanged"/>.
        /// Invokes the <see cref="OnHenLost"/> event.</remarks>
        private void HenDied(string message) {
            Destroy(this._playerManager.gameObject);

            if (this._waveSpawnCoroutine != null) {
                this.StopCoroutine(this._waveSpawnCoroutine);
            }

            this._dialogueRunner.Stop();
            this.HenLost(message);
        }

        /// <summary> Records analytics trap switching data. </summary>
        /// <param name="trapIndex"> The most recently selected trap index. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnSelectedTrapIndexChanged"/> event. </remarks>
        private void SelectedTrapIndexChanged(int trapIndex) {
            if (UGS_Analytics.Instance is null) {
                return;
            }

            var isAffordable = this._playerManager.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;
            UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
        }

        /// <summary> Records analytics trap deployment data. </summary>
        /// <param name="trapIndex"> The most recently selected trap index. </param>
        /// <remarks> Subscribed to the <see cref="PlayerController.OnTrapDeployed"/> event. </remarks>
        private void OnTrapDeployed(int trapIndex) {
            if (UGS_Analytics.Instance is null) {
                return;
            }

            UGS_Analytics.DeployTrapCustomEvent(trapIndex);
        }
        
        /// <summary> Records analytics hero death data. </summary>
        /// <param name="numberLives"> The number of lives remaining for the hero. </param>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
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

        /// <summary> Records analytics hero stuck data. </summary>
        /// <param name="xPos"> The hero's current position along the x-axis. </param>
        /// <param name="yPos"> The hero's current position along the y-axis. </param>
        /// <param name="zPos"> The hero's current position along the z-axis. </param>
        /// <remarks> Subscribed to the <see cref="HeroMovement.OnHeroIsStuck"/> event. </remarks>
        private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
            if (UGS_Analytics.Instance is null) {
                return;
            }
            UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
        }

        /// <summary> Disables player input and triggers the win UI. </summary>
        /// <remarks> Subscribed to the <see cref="Hero.OnGameOver"/> event. Invokes the <see cref="OnHenWon"/> event.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, does nothing.
        /// Otherwise, stops recording the player input and creates a file for the recorded input. </p></remarks>
        private void GameWon() {
            this._playerManager.PlayerController.DisablePlayerInput();
            this.OnHenWon?.Invoke("The Hero was stopped, good work Hendall!");
        }

        /// <summary> Disables the player input and hero movement, and triggers the loss UI. </summary>
        /// <remarks> Subscribed to the <see cref="EndgameTarget.OnHeroReachedEndgameTarget"/> event.
        /// Invokes the <see cref="OnHenLost"/> event. </remarks>
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
    }
}
