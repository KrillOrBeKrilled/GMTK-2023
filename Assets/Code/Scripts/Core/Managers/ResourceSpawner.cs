using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Traps;
using UnityEngine;
using Random = UnityEngine.Random;

//*******************************************************************************************
// ResourceSpawner
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Managers {
    /// <summary>
    /// A script for the resource spawner game object.
    /// Periodically spawns trap material game objects as pickups near the player.
    /// Whenever a <see cref="Hero"/> dies, also spawns associated trap materials at the
    /// position where that hero died.
    /// </summary>
    /// <remarks>
    /// For easy editing in the Unity Editor, this class uses several serializable classes.
    /// See those classes towards the bottom of this script.
    /// </remarks>
    public class ResourceSpawner : Singleton<ResourceSpawner> {
        [SerializeField] private ResourceDropPrefabs _dropPrefabs;
        [Tooltip("The region to the player's left that can spawn native resources.")]
        [SerializeField] private float _spawnLeftRadius;
        [Tooltip("The region to the player's right that can spawn native resources.")]
        [SerializeField] private float _spawnRightRadius;
        [Tooltip("The time interval for a native resource drop to spawn.")]
        [SerializeField] private float _spawnInterval;
        [Tooltip("The minimum number of resources that one hero can potentially drop.")]
        [SerializeField] private int _minimumHeroDrops;
        [Tooltip("The maximum number of resources that one hero can potentially drop.")]
        [SerializeField] private int _maximumHeroDrops;
        [Tooltip("The initial height where the natural resource drops spawn.")]
        [SerializeField] private float _levelDropOffset;
        [SerializeField] private float _dropUpwardForce;
        [SerializeField] private float _dropHorizontalForce;
        [SerializeField] private float _dropRotationForce;

        private List<ResourceDrop> _levelDrops;
        private List<HeroDrop> _heroDrops;
        private Dictionary<HeroType, (List<ResourceDrop> Drops, int TotalWeight)> _dropMap;
        private int _totalLevelDropWeight;
        private Transform _playerTransform;
        private Coroutine _levelDropCoroutine;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void OnDestroy() {
            Hero.OnHeroDeath -= SpawnHeroDrop;
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        /// <summary>
        /// Links the resource spawner with the player transform and core game system events required for proper
        /// execution. Initializes the resource drop map probabilities according to the level and hero data.
        /// </summary>
        /// <param name="playerTransform"> The transform of the player entity representation. </param>
        /// <param name="levelDrops"> The level drops list for the random Resource Drops. </param>
        /// <param name="heroDrops"> The hero drops list for heroes. </param>
        public void Initialize(Transform playerTransform, List<ResourceDrop> levelDrops, List<HeroDrop> heroDrops) {
            this._playerTransform = playerTransform;
            this._levelDrops = levelDrops;
            this._heroDrops = heroDrops;
            if (_playerTransform == null) {
                Debug.LogWarning("ResourceSpawner: Set Player Transform failed");
            }
            
            // Calculate the total weight only once here, also same for hero drops
            _totalLevelDropWeight = _levelDrops.Sum(drop => drop.weight);
            InitializeDropMap();
            
            Hero.OnHeroDeath += SpawnHeroDrop;
            EventManager.Instance.GameOverEvent.AddListener(StopLevelDropCoroutine);
        }

        /// <summary>
        /// Starts the spawner, and subscribe its counterpart for stopping the spawner
        /// to the <see cref="GameOverEvent"/>.
        /// </summary>
        public void StartSpawner() {
            _levelDropCoroutine = StartCoroutine(LevelDropCoroutine());
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods

        /// <summary>
        /// Populates the drop map bookkeeping structure with all the possible resource drops to be obtained from
        /// each hero Type and their summed weights to be used for later spawning randomization.
        /// </summary>
        private void InitializeDropMap() {
            _dropMap = new Dictionary<HeroType, (List<ResourceDrop>, int)>();
            foreach (var heroDrop in _heroDrops) {
                int totalWeight = heroDrop.drops.Sum(d => d.weight);
                _dropMap.Add(heroDrop.heroType, (heroDrop.drops, totalWeight));
            }
        }

        /// <summary>
        /// Infinitely spawns a resource drop every <see cref="_spawnInterval"/> duration of time.
        /// </summary>
        private IEnumerator LevelDropCoroutine() {
            while (true) {
                yield return new WaitForSeconds(_spawnInterval);
                SpawnLevelDrop();
            }
        }

        /// <summary>
        /// Disables the spawner. Subscribed to the <see cref="EventManager.GameOverEvent"/>.
        /// </summary>
        private void StopLevelDropCoroutine() {
            if (_levelDropCoroutine == null) {
                Debug.LogWarning("Level Drop Coroutine is not active.");
                return;
            }
            StopCoroutine(_levelDropCoroutine);
        }
        
        /// <summary>
        /// Chooses a resource Type from the list of drops associated with this level,
        /// and instantiate the resource prefab at some position near the player.
        /// Each drop in the list is weighted. The higher the weight, the more likely
        /// it is to drop.
        /// </summary>
        /// <remarks>
        /// Currently level drops spawn from the ceiling and drop down.
        /// </remarks>
        private void SpawnLevelDrop() {
            ResourceDrop drop = GetRandomDrop(_levelDrops, _totalLevelDropWeight);
            
            if (drop != null) {
                float spawnOffset = Random.Range(_spawnLeftRadius, _spawnRightRadius);
                var position = _playerTransform.position;
                Vector3 spawnPosition = new Vector3(position.x + spawnOffset, _levelDropOffset, position.z);
                ResourcePickup prefab = this._dropPrefabs.GetResourceDropPrefab(drop.resourceType).GetComponent<ResourcePickup>();
                var pickup = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
                pickup.Initialize(new ResourcePickupData(drop.resourceType, 1));
            }
        }

        /// <summary>
        /// Chooses a resource Type from the list of drops associated with the dead hero,
        /// and instantiate the resource prefab at the hero's position.
        /// Each drop in the list is weighted. The higher the weight, the more likely
        /// it is to drop.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDeath"/> event. </remarks>
        /// <param name="heroType"> The hero Type that died. </param>
        /// <param name="heroTransform"> The transform where the hero died. </param>
        private void SpawnHeroDrop(HeroType heroType, Transform heroTransform) {
            if (!_dropMap.ContainsKey(heroType)) {
                return;
            }

            var heroDrops = _dropMap[heroType];

            int dropAmount = Random.Range(_minimumHeroDrops, _maximumHeroDrops);
            for (int i = 0; i < dropAmount; i++) {
                ResourceDrop drop = GetRandomDrop(heroDrops.Drops, heroDrops.TotalWeight);
                ResourcePickup prefab = this._dropPrefabs.GetResourceDropPrefab(drop.resourceType).GetComponent<ResourcePickup>();
                if (drop != null) {
                    ResourcePickup pickup = Instantiate(prefab, heroTransform.position, Quaternion.identity, transform);
                    ApplyInitialForces(pickup);
                    pickup.Initialize(new ResourcePickupData(drop.resourceType, 1));
                }
            }
        }

        /// <summary>
        /// Selects a random resource drop from a given list, considering the weight of each drop.
        /// The higher the weight of a drop, the more likely it is to be selected.
        /// </summary>
        /// <param name="drops"> The list of resource drops to choose from. </param>
        /// <returns>
        /// A randomly selected ResourceDrop based on weight.
        /// Returns null if the list is empty or if no drop is selected.
        /// </returns>
        private ResourceDrop GetRandomDrop(List<ResourceDrop> drops, int totalWeight) {
            int randomNumber = Random.Range(0, totalWeight);
            ResourceDrop selected = null;

            foreach (var drop in drops) {
                if (randomNumber < drop.weight) {
                    selected = drop;
                    break;
                }
                randomNumber -= drop.weight;
            }

            return selected;
        }
        
        /// <summary>
        /// Flick the dropped material upwards randomly.
        /// </summary>
        /// <param name="pickup"> The pickup instance to apply the forces on. </param>
        private void ApplyInitialForces(ResourcePickup pickup) {
            float horizontalForce = Random.Range(-_dropHorizontalForce, _dropHorizontalForce);
            float rotationForce = Random.Range(_dropRotationForce, _dropRotationForce);
                
            pickup.SetRigidBodyVelocity(new Vector2(horizontalForce, _dropUpwardForce));
            pickup.SetRigidBodyAngularVelocity(rotationForce);
        }

        #endregion
    }
}