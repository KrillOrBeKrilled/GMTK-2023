using System.Linq;
using KrillOrBeKrilled.Common.Interfaces;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//*******************************************************************************************
// Trap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Abstract class that handles general logic for trap surveying, spawning, building,
    /// and collisions.
    /// </summary>
    /// <remarks>
    /// Trap setup, detonation, and impact on the hero are specific to the type of trap,
    /// and are left abstract to be implemented in subclasses.
    /// </remarks>
    public abstract class Trap : MonoBehaviour, ITrap {
        [Tooltip("The required resources to deploy this trap managed by the ResourceManager.")]
        [SerializeField] protected List<ResourceEntry> RecipeList;
        [Tooltip("Tilemap position offsets to specify the tiles needed for deployment of this trap calculated from " +
                 "an origin in the TrapController.")]
        [SerializeField] protected List<TrapGridPoint> LeftGridPoints, RightGridPoints;
        [Tooltip("The minimum requirement of overlapping TrapTile types when deploying this trap.")]
        [SerializeField] protected int ValidationScore;
        [Tooltip("Adjusts trap component positions for clean animations and spawning customized for different trap sizes.")]
        [SerializeField] protected Vector3 LeftSpawnOffset, RightSpawnOffset, AnimationOffset;
        [Tooltip("The length of time required to build the trap when the player stands idle and in range.")]
        [SerializeField] protected float BuildingDuration;
        [Tooltip("A slider bar prefab to signify the trap build completion status.")]
        [SerializeField] protected GameObject SliderBar;

        public bool IsCeilingTrap;

        protected Vector3 SpawnPosition;
        protected Slider BuildCompletionBar;
        protected TrapSoundsController SoundsController;
        protected float ConstructionCompletion, t;
        protected bool IsBuilding, IsReady;

        private UnityAction _onDestroy;
        private Dictionary<ResourceType, int> _recipe;
        public Dictionary<ResourceType, int> Recipe {
            get {
                if (_recipe == null) {
                    InitializeRecipe();
                }
                return _recipe;
            }
        }

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        public void Update() {
            if (this.IsReady || !this.IsBuilding) {
                return;
            }

            this.ConstructionCompletion = Mathf.Lerp(this.ConstructionCompletion, 1f, this.t / this.BuildingDuration);
            this.t += Time.deltaTime;

            this.BuildCompletionBar.DOValue(this.ConstructionCompletion, 0.1f);

            // Make construction animations
            BuildTrap();
            
            if (!(this.ConstructionCompletion >= 0.99f)) {
                return;
            }
            
            // Set up trap once the construction has completed
            this.IsReady = true;
            this.IsBuilding = false;
            this.SoundsController.OnBuild(false);
            this.SoundsController.OnBuildComplete();
            Destroy(this.BuildCompletionBar.gameObject);
            
            SetUpTrap();
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Hero") && other.TryGetComponent(out IDamageable actor)) {
                this.OnEnteredTrap(actor);
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D other) {
            if (IsReady) {
                return;
            }
            
            ITrapBuilder actor;
            if (other.CompareTag("Builder Range")) {
                actor = other.GetComponentInParent<ITrapBuilder>();
            } else if (!other.TryGetComponent(out actor)) {
                return;
            }
            
            this.IsBuilding = actor.CanBuildTrap();
            this.SoundsController.OnBuild(this.IsBuilding);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Builder Range")) {
                this.IsBuilding = false;
                this.SoundsController.OnBuild(this.IsBuilding);
                return;
            }

            if (other.TryGetComponent(out IDamageable damageActor)) {
                this.OnExitedTrap(damageActor);
            }
        }

        private void OnDestroy() {
            this._onDestroy?.Invoke();
        }

        #endregion

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region ITrap Implementations

        public bool IsTrapReady() {
            return IsReady;
        }
        
        #endregion
        
        #region Trap Deployment
        
        /// <summary>
        /// Acts as a constructor, initializing all bookkeeping data upon trap prefab instantiation. Upon trap
        /// deployment, destroys level tiles for traps built into the ground, creates trap build completion UI,
        /// and executes a trap deployment animation.
        /// </summary>
        /// <param name="spawnPosition"> The target spawn position for this trap. </param>
        /// <param name="canvas"> The canvas to spawn trap UI. </param>
        /// <param name="soundsController"> The controller used to play all trap-related SFX. </param>
        /// <param name="onDestroy"> A delegate used to reset tiles when trap is destroyed. </param>
        public virtual void Construct(Vector3 spawnPosition, Canvas canvas, 
            TrapSoundsController soundsController, UnityAction onDestroy = null) {
            // Initialize all the bookkeeping structures we will need
            SpawnPosition = spawnPosition;
            SoundsController = soundsController;
            _onDestroy = onDestroy;
            
            // Spawn a slider to indicate the progress on the build
            GameObject sliderObject = Instantiate(this.SliderBar, canvas.transform);

            var offsetDirection = IsCeilingTrap ? -1 : 1;
            sliderObject.transform.position = spawnPosition + this.AnimationOffset + Vector3.up * offsetDirection;
            this.BuildCompletionBar = sliderObject.GetComponent<Slider>();

            // Trap deployment visuals
            // Slide down trap and fade it into existence
            this.transform.position = spawnPosition + Vector3.up * (offsetDirection * 3f);
            this.transform.DOMove(spawnPosition + Vector3.up * this.AnimationOffset.y, 0.2f);

            var sprite = GetComponent<SpriteRenderer>();
            var color = sprite.color;
            sprite.color = new Color(color.r, color.g, color.b, 0);
            sprite.DOFade(1, 0.4f);

            // Initiate the build time countdown
            this.ConstructionCompletion = 0;
        }
        
        /// <summary>
        /// Adjusts the trap spawn position relative to an origin position when deploying a trap to the left of
        /// the player, facing the negative direction along the x-axis.
        /// </summary>
        /// <param name="origin"> The central position for the trap to be spawned. </param>
        /// <returns> The resulting trap spawn position after the application of offset adjustments. </returns>
        public Vector3 GetLeftSpawnPoint(Vector3 origin) {
            return origin + this.LeftSpawnOffset;
        }

        /// <summary>
        /// Adjusts the trap spawn position relative to an origin position when deploying a trap to the right of
        /// the player, facing the positive direction along the x-axis.
        /// </summary>
        /// <param name="origin"> The central position for the trap to be spawned. </param>
        /// <returns> The resulting trap spawn position after the application of offset adjustments. </returns>
        public Vector3 GetRightSpawnPoint(Vector3 origin) {
            return origin + this.RightSpawnOffset;
        }

        #endregion
        
        #region Trap Surveying
        
        /// <summary>
        /// Retrieves the tilemap grid offset points for trap deployment when the player faces the negative
        /// direction along the x-axis.
        /// </summary>
        /// <returns> A list of tilemap offsets to pinpoint tiles needed for trap deployment. </returns>
        /// <remarks> Depending on the shape and extent of the trap, the offsets will vary between trap types. </remarks>
        public List<TrapGridPoint> GetLeftGridPoints() {
            return this.LeftGridPoints;
        }

        /// <summary>
        /// Retrieves the tilemap grid offset points for trap deployment when the player faces the positive
        /// direction along the x-axis.
        /// </summary>
        /// <returns> A list of tilemap offsets to pinpoint tiles needed for trap deployment. </returns>
        /// <remarks> Depending on the shape and extent of the trap, the offsets will vary between trap types. </remarks>
        public List<TrapGridPoint> GetRightGridPoints() {
            return this.RightGridPoints;
        }

        /// <summary>
        /// Checks that a score reaches the <see cref="ValidationScore"/> threshold required to deploy this trap.
        /// </summary>
        /// <param name="score"> The current validation score to compare against the requirement. </param>
        /// <returns> If the score is greater than or equal to the <see cref="ValidationScore"/>. </returns>
        /// <remarks>
        /// A <see cref="ValidationScore"/> is the minimum number of TrapTile tile types that this trap must overlap
        /// to be successfully deployed. Depending on the shape and extent of the trap, this validation score
        /// will vary between trap types.
        /// </remarks>
        public bool IsValidScore(int score) {
            return score >= this.ValidationScore;
        }
        
        #endregion
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        #region Trap Initialization

        /// <summary>
        /// Initialize the resource recipe dictionary.
        /// </summary>
        protected void InitializeRecipe() {
            _recipe = RecipeList.ToDictionary(entry => entry.type, entry => entry.amount);
        }
        
        #endregion
        
        #region Trap Building
        
        /// <summary>
        /// Applies an animation to the trap while it's being built.
        /// </summary>
        /// <remarks> Invoked every frame that the trap is being built. </remarks>
        protected virtual void BuildTrap() {
            // Randomly shake the trap along the x and y-axis
            Vector3 targetPosition = new Vector3(this.SpawnPosition.x + Random.Range(-0.5f, 0.5f),
                this.SpawnPosition.y + Random.Range(0.01f, 0.2f));

            // Shake out then back
            transform.DOMove(targetPosition, 0.05f);
            transform.DOMove(this.SpawnPosition + Vector3.up * this.AnimationOffset.y, 0.05f);
        }
        
        /// <summary>
        /// Applies any SFX, animations, updates to physics, and logic associated with the build completion of a trap,
        /// when a trap stands ready for detonation. 
        /// </summary>
        protected abstract void SetUpTrap();
        
        #endregion
        
        #region Trap Detonation
        
        /// <summary>
        /// Applies any SFX, animations, updates to physics, and logic to the trap when it is being detonated,
        /// or unleashed on an <see cref="IDamageable"/> actor. 
        /// </summary>
        protected abstract void DetonateTrap();

        /// <summary>
        /// Cleans up the trap data and frees the used trap tiles upon the completion of the trap detonation animation.
        /// </summary>
        protected virtual void OnDetonateTrapAnimationCompete() {}
        
        /// <summary>
        /// Applies resulting effects and reactions to the <see cref="IDamageable"/> actor upon trap detonation. 
        /// </summary>
        /// <remarks> Invoked upon triggered collision with a hero tagged GameObject. </remarks>
        protected abstract void OnEnteredTrap(IDamageable actor);
        
        /// <summary>
        /// Applies lasting effects and reactions to the <see cref="IDamageable"/> actor when exiting the trap collider. 
        /// </summary>
        /// <remarks> Invoked upon exiting a trigger collision with a hero tagged GameObject.. </remarks>
        protected abstract void OnExitedTrap(IDamageable actor);
        
        #endregion
        
        #endregion
    }
}
