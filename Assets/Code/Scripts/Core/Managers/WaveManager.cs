using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Environment;
using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Yarn.Unity;

namespace KrillOrBeKrilled.Core.Managers {
    public class WaveManager: MonoBehaviour {
        [Header("Heroes")]
        [SerializeField] private Hero _defaultHeroPrefab;
        [SerializeField] private Hero _druidHeroPrefab;
        [SerializeField] private RespawnPoint _respawnPointPrefab;

        [Tooltip("Tracks when a hero is spawned.")]
        public UnityEvent<Hero> OnHeroSpawned { get; private set; }

        [Header("Wave Events")]
        [SerializeField] private GameEvent _onAllWavesCleared;
        [SerializeField] private GameEvent _onWaveCleared;
        [SerializeField] private IntEvent _newWaveLeftCount;

        [Header("Actor Events")] 
        [SerializeField] private HeroEvent _onHeroActorSpawn;
        [SerializeField] private HeroEvent _onHeroActorDestroy;
        
        private Queue<WaveData> _nextWavesDataQueue;
        private Queue<WaveData> _lastWavesDataQueue;
        private bool _isEndlessLevel;
        private HeroSoundsController _heroSoundsController;
        private Tilemap _levelTilemap;
        
        private IEnumerator _waveSpawnCoroutine;

        private Hero _heroActor;
        
        // ------------- Wave Spawning ---------------
        /// Tracks the active heroes on the level map at any given time.
        private readonly List<Hero> _heroes = new();

        /// Tracks the active hero respawn points at any given time.
        private readonly List<RespawnPoint> _respawnPoints = new();
        /// Spawns heroes actively throughout the level gameplay.
        private RespawnPoint _activeRespawnPoint;
        
        private const float EndlessLevelHealthIncreaseRate = 1.5f;

        public void Initialize(LevelData levelData, 
                               HeroSoundsController heroSoundsController,
                               Tilemap tilemap) {
            this.OnHeroSpawned = new UnityEvent<Hero>();
            
            this._nextWavesDataQueue = new Queue<WaveData>(levelData.WavesData.WavesList);
            this._lastWavesDataQueue = new Queue<WaveData>(levelData.WavesData.WavesList);
            this._isEndlessLevel = levelData.Type == LevelData.LevelType.Endless;
            this._heroSoundsController = heroSoundsController;
            this._levelTilemap = tilemap;
            
            foreach (Vector3 respawnPosition in levelData.RespawnPositions) {
                RespawnPoint newPoint = Instantiate(this._respawnPointPrefab, respawnPosition, 
                                                    Quaternion.identity, this.transform);
                this._respawnPoints.Add(newPoint);
            }

            this._activeRespawnPoint = this._respawnPoints.First();
        }

        public void StopWaves() {
            if (this._waveSpawnCoroutine != null) {
                this.StopCoroutine(this._waveSpawnCoroutine);
            }
        }
        
        /// <summary>
        /// Disables movement for every hero currently active in the level.
        /// </summary>
        public void StopAllHeroes() {
            foreach (var hero in this._heroes) {
                hero.StopRunning();
            }
        }

        /// <summary>
        /// Freezes all heroes' actions and movement in the level.
        /// </summary>
        public void FreezeAllHeroes() {
            foreach (Hero hero in this._heroes) {
                hero.Freeze();
            }
        }

        /// <summary>
        /// Unfreezes all heroes' actions and movement in the level.
        /// </summary>
        public void UnfreezeAllHeroes() {
            foreach (Hero hero in this._heroes) {
                hero.Unfreeze();
            }
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
                WaveData newWave = new() {
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

        /// <summary>
        /// Instantiates a new hero according to the <see cref="HeroData.Type"/> at the
        /// <see cref="_activeRespawnPoint"/> and registers the hero in bookkeeping structures, event listeners,
        /// and the game UI.
        /// </summary>
        /// <param name="heroData"> The data associated with the hero to spawn stored within each
        ///     <see cref="WaveData"/>. </param>
        /// <returns> The instantiated hero GameObject. </returns>
        /// <remarks> Invokes the <see cref="OnHeroSpawned"/> event. </remarks>
        private void SpawnHero(HeroData heroData) {
            Hero heroPrefab = this._defaultHeroPrefab;
            if (heroData.Type == HeroType.Druid) {
                heroPrefab = this._druidHeroPrefab;
            }

            Hero newHero = Instantiate(heroPrefab, this._activeRespawnPoint.transform);
            newHero.Initialize(heroData, this._heroSoundsController, this._levelTilemap);
            newHero.OnHeroDied.AddListener(this.OnHeroDied);

            this._heroes.Add(newHero);
            this.OnHeroSpawned?.Invoke(newHero);
        }

        /// <summary>
        /// Parses the next <see cref="WaveData"/> to spawn all the associated heroes in intervals denoted by the
        /// <see cref="WaveData.HeroSpawnDelayInSeconds"/>. Spawns the next wave if
        /// <see cref="WaveData.NextWaveSpawnDelayInSeconds"/> is nonzero and nonnegative, otherwise waits until
        /// the previous wave is defeated before spawning the next wave.
        /// </summary>
        /// <remarks>
        /// If the registered <see cref="WaveData"/>
        /// are all completed and the <see cref="LevelData.LevelType"/> is set to
        /// <see cref="LevelData.LevelType.Endless"/>, generates new <see cref="WaveData"/> with scaled health
        /// values. Otherwise, the wave spawner is aborted and the level is completed.
        /// </remarks>
        private IEnumerator SpawnNextWave() {
            if (this._nextWavesDataQueue.Count <= 0) {
                if (!this._isEndlessLevel) {
                    yield break;
                }

                this.GenerateNextWaves();
            }

            this._newWaveLeftCount.Raise(this._nextWavesDataQueue.Count);
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
        /// Starts the <see cref="SpawnNextWave"/> coroutine to begin the enemy spawning gameplay.
        /// </summary>
        public void StartWaveSpawning() {
            this._waveSpawnCoroutine = this.SpawnNextWave();
            this.StartCoroutine(this._waveSpawnCoroutine);
        }
    
        /// <summary>
        /// Spawns a new hero from the level data at the corresponding spawn point.
        /// </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("spawn_hero_actor")]
        public void SpawnHeroActor() {
            Hero heroPrefab = this._defaultHeroPrefab;
            this._heroActor = Instantiate(heroPrefab, this._activeRespawnPoint.transform);
            this._heroActor.Initialize(HeroData.DefaultHero, this._heroSoundsController, this._levelTilemap);
            this._onHeroActorSpawn.Raise(this._heroActor);
        }
        
        /// <summary>
        /// Triggers the sequence to make the hero enter the level.
        /// </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("enter_hero_actor")]
        public void EnterHero() {
            this._heroActor.EnterLevel();
        }
        
        /// <summary>
        /// Triggers the sequence to make the hero exit the level.
        /// </summary>
        /// <remarks> Can be accessed as a YarnCommand. </remarks>
        [YarnCommand("exit_hero_actor")]
        public void ExitHero() {
            this._onHeroActorDestroy.Raise(this._heroActor);
            this._heroActor.ExitLevel();
        }
        
        /// <summary>
        /// Records analytics hero death data.
        /// </summary>
        /// <param name="hero"> The hero that died. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
        private void OnHeroDied(Hero hero) {
            this._heroes.Remove(hero);

            bool noMoreWaves = !this._isEndlessLevel && this._nextWavesDataQueue.Count <= 0;
            bool isWaveCleared = this._heroes.Count <= 0;
            if (noMoreWaves && isWaveCleared) {
                this._onAllWavesCleared.Raise();
            } else if (isWaveCleared) {
                this._onWaveCleared.Raise();
                this._waveSpawnCoroutine = this.SpawnNextWave();
                this.StartCoroutine(this._waveSpawnCoroutine);
            }
        }
    }
}