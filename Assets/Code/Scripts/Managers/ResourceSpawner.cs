using System;
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
namespace KrillOrBeKrilled.Managers {
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
        [Tooltip("The list of native resources on this level.")]
        [SerializeField] private List<ResourceDrop> _levelDrops;
        [Tooltip("The region to the player's left that can spawn native resources.")]
        [SerializeField] private float _spawnLeftRadius;
        [Tooltip("The region to the player's right that can spawn native resources.")]
        [SerializeField] private float _spawnRightRadius;
        [Tooltip("The time interval for a native resource drop to spawn.")]
        [SerializeField] private float _spawnInterval;
        [Tooltip("The list of heroes and their list of resources on this level.")]
        [SerializeField] private List<HeroDrop> _heroDrops;
        [Tooltip("The minimum number of resources that one hero can potentially drop.")]
        [SerializeField] private int _minimumHeroDrops;
        [Tooltip("The maximum number of resources that one hero can potentially drop.")]
        [SerializeField] private int _maximumHeroDrops;
        [Tooltip("The initial height where the natural resource drops spawn.")]
        [SerializeField] private float _levelDropOffset;
        [SerializeField] private float _dropUpwardForce;
        [SerializeField] private float _dropHorizontalForce;
        [SerializeField] private float _dropRotationForce;

        private Dictionary<HeroData.HeroType, (List<ResourceDrop> Drops, int TotalWeight)> _dropMap;
        private int _totalLevelDropWeight;
        private Transform _playerTransform;
        private Coroutine _levelDropCoroutine;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Start() {
            // Calculate the total weight only once here, also same for hero drops
            _totalLevelDropWeight = _levelDrops.Sum(drop => drop.weight);
            InitializeDropMap();
            
            Hero.OnHeroDeath += SpawnHeroDrop;
            EventManager.Instance.GameOverEvent.AddListener(StopLevelDropCoroutine);
        }

        private void OnDestroy() {
            Hero.OnHeroDeath -= SpawnHeroDrop;
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        /// <summary>
        /// Starts the spawner, and subscribe its counterpart for stopping the spawner
        /// to the <see cref="GameOverEvent"/>.
        /// </summary>
        public void StartSpawner() {
            _levelDropCoroutine = StartCoroutine(LevelDropCoroutine());
        }
        
        public void SetPlayerTransform(Transform playerTransform) {
            _playerTransform = playerTransform;
            if (_playerTransform == null) {
                Debug.LogWarning("ResourceSpawner: Set Player Transform failed");
            }
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods

        private void InitializeDropMap() {
            _dropMap = new Dictionary<HeroData.HeroType, (List<ResourceDrop>, int)>();
            foreach (var heroDrop in _heroDrops) {
                int totalWeight = heroDrop.drops.Sum(d => d.weight);
                _dropMap.Add(heroDrop.heroType, (heroDrop.drops, totalWeight));
            }
        }

        private IEnumerator LevelDropCoroutine() {
            while (true) {
                yield return new WaitForSeconds(_spawnInterval);
                SpawnLevelDrop();
            }
        }

        /// <summary>
        /// Disables the spawner. Subscribed to <see cref="GameOverEvent"/>.
        /// </summary>
        private void StopLevelDropCoroutine() {
            if (_levelDropCoroutine == null) {
                Debug.LogWarning("Level Drop Coroutine is not active.");
                return;
            }
            StopCoroutine(_levelDropCoroutine);
        }
        
        /// <summary>
        /// Chooses a resource type from the list of drops associated with this level,
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
                var pickup = Instantiate(drop.resourcePrefab, spawnPosition, Quaternion.identity, transform);
            }
        }

        /// <summary>
        /// Chooses a resource type from the list of drops associated with the dead hero,
        /// and instantiate the resource prefab at the hero's position.
        /// Each drop in the list is weighted. The higher the weight, the more likely
        /// it is to drop.
        /// </summary>
        /// <param name="heroType"> The hero type that died. </param>
        /// <param name="heroTransform"> The transform where the hero died. </param>
        private void SpawnHeroDrop(HeroData.HeroType heroType, Transform heroTransform) {
            if (!_dropMap.ContainsKey(heroType)) {
                Debug.LogWarning("Cannot find Hero Type in ResourceSpawner");
                return;
            }

            var heroDrops = _dropMap[heroType];

            int dropAmount = Random.Range(_minimumHeroDrops, _maximumHeroDrops);
            for (int i = 0; i < dropAmount; i++) {
                ResourceDrop drop = GetRandomDrop(heroDrops.Drops, heroDrops.TotalWeight);
                if (drop != null) {
                    var pickup = Instantiate(drop.resourcePrefab, heroTransform.position, Quaternion.identity,
                        transform);
                    ApplyInitialForces(pickup);
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
    

    //*******************************************************************************************
    // ResourceDrop
    //*******************************************************************************************
    /// <summary>
    /// Stores a particular trap material's type, its prefab, and a drop weight (higher weight
    /// means more likely to drop).
    /// </summary>
    [Serializable]
    public class ResourceDrop {
        [Tooltip("The type of the dropped resource.")]
        public ResourceType resourceType;
        [Tooltip("The prefab for the resource pickup.")]
        public ResourcePickup resourcePrefab;
        [Tooltip("The higher the weight, the more likely to drop.")]
        public int weight;
    }
    
    //*******************************************************************************************
    // HeroDrop
    //*******************************************************************************************
    /// <summary>
    /// Stores a hero type and a list of potential resource drops.
    /// </summary>
    [Serializable]
    public class HeroDrop {
        [Tooltip("The hero type.")]
        public HeroData.HeroType heroType;
        [Tooltip("The list of potential resource drops.")]
        public List<ResourceDrop> drops;
    }
}