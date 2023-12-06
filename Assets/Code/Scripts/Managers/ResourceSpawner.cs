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
    public class ResourceSpawner : MonoBehaviour {
        [Tooltip("The list of native resources on this level.")]
        [SerializeField] private List<ResourceDrop> _levelDrops;
        [Tooltip("The radius of the drop range centred on the player.")]
        [SerializeField] private float _spawnRadius;
        [Tooltip("The time interval for a native resource drop to spawn.")]
        [SerializeField] private float _spawnInterval;
        [Tooltip("The list of heroes and their list of resources on this level.")]
        [SerializeField] private List<HeroDrop> _heroDrops;
        [Tooltip("How many resources can be dropped from one hero.")]
        [SerializeField] private int _heroDropAmount;
        [Tooltip("The initial height where the natural resource drops spawn.")]
        [SerializeField] private float _levelDropOffset;
        [SerializeField] private float _dropUpwardForce;
        [SerializeField] private float _dropHorizontalForce;
        [SerializeField] private float _dropRotationForce;

        private Dictionary<HeroData.HeroType, List<ResourceDrop>> _dropMap;
        private Transform _playerTransform;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            InitializeDropMap();
            StartCoroutine(LevelDropCoroutine());
        }

        private void Start() {
            Hero.OnHeroDeath += SpawnHeroDrop;
        }

        private void OnDestroy() {
            Hero.OnHeroDeath -= SpawnHeroDrop;
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        public void SetPlayerTransform(Transform playerTransform) {
            _playerTransform = playerTransform;
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods

        private void InitializeDropMap() {
            _dropMap = new Dictionary<HeroData.HeroType, List<ResourceDrop>>();
            foreach (var drop in _heroDrops) {
                _dropMap.Add(drop.heroType, drop.drops);
            }
        }

        private IEnumerator LevelDropCoroutine() {
            while (true) {
                yield return new WaitForSeconds(_spawnInterval);
                if (_playerTransform != null) {
                    SpawnLevelDrop();
                }
            }
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
            ResourceDrop drop = GetRandomDrop(_levelDrops);
            
            if (drop != null) {
                float spawnOffset = Random.Range(0, _spawnRadius);
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
            if (!_dropMap.ContainsKey(heroType)) return;

            var heroDrops = _dropMap[heroType];

            for (int i = 0; i < _heroDropAmount; i++) {
                ResourceDrop drop = GetRandomDrop(heroDrops);
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
        private ResourceDrop GetRandomDrop(List<ResourceDrop> drops) {
            int totalWeight = drops.Sum(drop => drop.weight);
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
            if (pickup.Rigidbody2D != null) {
                float horizontalForce = Random.Range(-_dropHorizontalForce, _dropHorizontalForce);
                float rotationForce = Random.Range(_dropRotationForce, _dropRotationForce);
                
                pickup.Rigidbody2D.velocity = new Vector2(horizontalForce, _dropUpwardForce);
                pickup.Rigidbody2D.angularVelocity = rotationForce;
            }
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