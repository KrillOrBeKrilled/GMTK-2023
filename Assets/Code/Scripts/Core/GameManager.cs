using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Cameras;
using KrillOrBeKrilled.Common.Commands;
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
    public class GameManager : MonoBehaviour {
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

        /// The instantiated endgameTargetPrefab.
        private EndgameTarget _endgameTarget;
        /// The instantiated defaultHeroPrefab used for story mode dialogue sequences.
        private Hero _heroActor;

        // ------------- Wave Spawning ---------------
        /// Tracks the active heroes on the level map at any given time.
        private readonly List<Hero> _heroes = new List<Hero>();

        /// Tracks the active hero respawn points at any given time.
        private readonly List<RespawnPoint> _respawnPoints = new List<RespawnPoint>();
        /// Spawns a hero used for story mode dialogue sequences.
        private RespawnPoint _firstRespawnPoint;
        /// Spawns heroes actively throughout the level gameplay.
        private RespawnPoint _activeRespawnPoint;

        /// Contains all data on the waves and hero settings to spawn per wave that constitutes a playable level.
        private LevelData _levelData;
        private Queue<WaveData> _nextWavesDataQueue;
        private Queue<WaveData> _lastWavesDataQueue;
        private bool IsEndlessLevel => this._levelData != null && (this._levelData.Type == LevelData.LevelType.Endless);

        private const float EndlessLevelHealthIncreaseRate = 1.5f;

        private IEnumerator _waveSpawnCoroutine = null;

        // ------------- Sound Effects ---------------
        private HeroSoundsController _heroSoundsController;

        // ----------------- Events ------------------
        [Tooltip("Tracks when hero and UI data are set up and this manager sets up other critical gameplay system " +
                 "references.")]
        internal UnityEvent OnSetupComplete { get; private set; }
        [Tooltip("Tracks when the main gameplay loop begins.")]
        internal UnityEvent OnStartLevel { get; private set; }
        [Tooltip("Tracks when the player beats the game.")]
        internal UnityEvent<string> OnHenWon { get; private set; }
        [Tooltip("Tracks when the player loses the game.")]
        internal UnityEvent<string> OnHenLost { get; private set; }
        internal UnityEvent<Hero> OnHeroSpawned { get; private set; }

        private void Awake() {
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
                this._firstRespawnPoint.transform, this._endgameTarget.transform,
                this._playerManager.PlayerController.SetTrap);

            this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
            this._playerManager.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
            this._playerManager.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
            this._playerManager.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);
            this._playerManager.PlayerController.Initialize(this);

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

#region Scene Management
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
#endregion

        //========================================
        // Level Sequence
        //========================================

#region Level Sequence
        /// <summary> Spawns a new hero from the level data at the corresponding spawn point. </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
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

        /// <summary>
        /// Aborts the dialogue player if the dialogue is actively running and starts the level.
        /// </summary>
        private void SkipDialogue() {
            this.StartCoroutine(this.SkipDialogueCoroutine());
        }

        /// <summary>
        /// Aborts the dialogue player if the dialogue is actively running and immediately focuses the camera
        /// on the player before beginning the level spawners and gameplay.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="SkipDialogue"/>. </remarks>
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

        /// <summary>
        /// Skips the dialogue associated with the level story event if the <see cref="PlayerPrefsManager"/>
        /// settings are enabled. Otherwise, begins playing the level dialogue.
        /// </summary>
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

        /// <summary> Skips the dialogue for the level. </summary>
        /// <remarks> The endless level settings will skip story dialogue by default. </remarks>
        private void StartEndlessLevel() {
            this.SkipDialogue();
        }
#endregion

        //========================================
        // Wave Spawning
        //========================================

#region Wave Spawning
        /// <summary>
        /// Parses the next <see cref="WaveData"/> to spawn all the associated heroes in intervals denoted by the
        /// <see cref="WaveData.HeroSpawnDelayInSeconds"/>. Spawns the next wave if
        /// <see cref="WaveData.NextWaveSpawnDelayInSeconds"/> is nonzero and nonnegative, otherwise waits until
        /// the previous wave is defeated before spawning the next wave.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="SkipDialogue"/>. If the registered <see cref="WaveData"/>
        /// are all completed and the <see cref="LevelData.LevelType"/> is set to
        /// <see cref="LevelData.LevelType.Endless"/>, generates new <see cref="WaveData"/> with scaled health
        /// values. Otherwise, the wave spawner is aborted and the level is completed. </remarks>
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

        /// <summary>
        /// Instantiates a new hero according to the <see cref="HeroData.Type"/> at the
        /// <see cref="_activeRespawnPoint"/> and registers the hero in bookkeeping structures, event listeners,
        /// and the game UI.
        /// </summary>
        /// <param name="heroData"> The data associated with the hero to spawn stored within each
        /// <see cref="WaveData"/>. </param>
        /// <returns> The instantiated hero GameObject. </returns>
        /// <remarks> Invokes the <see cref="OnHeroSpawned"/> event. </remarks>
        private Hero SpawnHero(HeroData heroData) {
            var heroPrefab = this._defaultHeroPrefab;
            if (heroData.Type == HeroData.HeroType.Druid) {
                heroPrefab = this._druidHeroPrefab;
            }

            var newHero = Instantiate(heroPrefab, this._activeRespawnPoint.transform);
            newHero.Initialize(heroData, _heroSoundsController);
            newHero.OnHeroDied.AddListener(this.OnHeroDied);
            newHero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

            this._heroes.Add(newHero);
            this.OnHeroSpawned.Invoke(newHero);
            return newHero;
        }

        /// <summary>
        /// Generates a new Queue of <see cref="WaveData"/> containing identical <see cref="WaveData"/> and
        /// <see cref="HeroData"/> to the previous Queue, but with scaled <see cref="HeroData.Health"/> values
        /// associated with the progress in the level.
        /// </summary>
        /// <remarks> Invoked for the <see cref="LevelData.LevelType.Endless"/> mode only. </remarks>
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
#endregion

        //========================================
        // Event Listeners: UGSAnalytics & Game Over
        //========================================

#region Event Listeners
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
        /// <remarks> Helper for <see cref="GameManager.OnPlayerStateChanged"/>. </remarks>
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

        /// <summary> Disables the player input and hero movement, and triggers the loss UI. </summary>
        /// <remarks> Subscribed to the <see cref="EndgameTarget.OnHeroReachedEndgameTarget"/> event. </remarks>
        private void HeroReachedLevelEnd() {
            this.HenLost("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
        }

        /// <summary>
        /// Disables the player input and wave spawner, and disables the hero movement before triggering the win UI.
        /// </summary>
        /// <param name="endgameMessage"> The message to be displayed on the loss UI. </param>
        /// <remarks> Invokes the <see cref="OnHenLost"/> event. </remarks>
        private void HenLost(string endgameMessage) {
            this._playerManager.PlayerController.DisablePlayerInput();

            if (this._waveSpawnCoroutine != null) {
                this.StopCoroutine(this._waveSpawnCoroutine);
            }

            this.StopAllHeroes();
            this.OnHenLost?.Invoke(endgameMessage);
        }

        /// <summary> Disables movement for every hero currently active in the level. </summary>
        private void StopAllHeroes() {
            foreach (var hero in this._heroes) {
                hero.HeroMovement.ToggleMoving(false);
            }
        }

        /// <summary> Disables and freezes player input and triggers the win UI. </summary>
        /// <param name="message"> The message to be displayed on the win UI. </param>
        /// <remarks> Invokes the <see cref="OnHenWon"/> event.
        /// <p> If the <see cref="PlayerController"/> refers to the <see cref="RecordingController"/>, does nothing.
        /// Otherwise, stops recording the player input and creates a file for the recorded input. </p></remarks>
        private void HenWon(string message) {
            this._playerManager.PlayerController.DisablePlayerInput();
            var freezeCommand = new FreezeCommand(this._playerManager.PlayerController);
            this._playerManager.PlayerController.ExecuteCommand(freezeCommand);
            this.OnHenWon?.Invoke(message);
        }
#endregion
    }
}
