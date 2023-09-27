using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Managers;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// Trap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Abstract class that handles general logic for trap surveying, spawning, building,
    /// and collisions.
    /// </summary>
    /// <remarks>
    /// Trap setup, detonation, and impact on the <see cref="Hero"/> are
    /// specific to the type of trap, and are left abstract to be implemented in
    /// subclasses.
    /// </remarks>
    public abstract class Trap : MonoBehaviour {
        [Tooltip("The cost to deploy this trap in coins managed by the CoinManager.")]
        [SerializeField] public int Cost;
        [Tooltip("Tilemap position offsets to specify the tiles needed for deployment of this trap calculated from " +
                 "an origin in the TrapController.")]
        [SerializeField] protected List<Vector3Int> LeftGridPoints, RightGridPoints;
        [Tooltip("The minimum requirement of overlapping TrapTile types when deploying this trap.")]
        [SerializeField] protected int ValidationScore;
        [Tooltip("Adjusts trap component positions for clean animations and spawning customized for different trap sizes.")]
        [SerializeField] protected Vector3 LeftSpawnOffset, RightSpawnOffset, AnimationOffset;
        [Tooltip("The length of time required to build the trap when the player stands idle and in range.")]
        [SerializeField] protected float BuildingDuration;
        [Tooltip("A slider bar prefab to signify the trap build completion status.")]
        [SerializeField] protected GameObject SliderBar;

        protected Vector3 SpawnPosition;
        protected Vector3Int[] TilePositions;
        protected Slider BuildCompletionBar;
        protected TrapSoundsController SoundsController;
        protected float ConstructionCompletion, t;
        protected bool IsBuilding, IsReady;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        public void Update() {
            if (!IsReady && IsBuilding) {
                ConstructionCompletion = Mathf.Lerp(ConstructionCompletion, 1f, t / BuildingDuration);
                t += Time.deltaTime;

                BuildCompletionBar.DOValue(ConstructionCompletion, 0.1f);

                // Make construction animations
                BuildTrap();

                if (ConstructionCompletion >= 0.99f) {
                    IsReady = true;
                    IsBuilding = false;
                    SoundsController.OnBuild(false);
                    SoundsController.OnBuildComplete();
                    Destroy(BuildCompletionBar.gameObject);

                    // Play trap set up animation
                    SetUpTrap();
                }
            }
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
            
            IsBuilding = actor.CanBuildTrap();
            SoundsController.OnBuild(IsBuilding);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Builder Range")) {
                IsBuilding = false;
                SoundsController.OnBuild(IsBuilding);
                return;
            }

            if (other.TryGetComponent(out IDamageable damageActor)) {
                this.OnExitedTrap(damageActor);
            }
        }
        
        #endregion

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region Trap Deployment
        
        /// <summary>
        /// Acts as a constructor, initializing all bookkeeping data upon trap prefab instantiation. Upon trap
        /// deployment, destroys level tiles for traps built into the ground, creates trap build completion UI,
        /// and executes a trap deployment animation.
        /// </summary>
        /// <param name="spawnPosition"> The target spawn position for this trap. </param>
        /// <param name="canvas"> The canvas to spawn trap UI. </param>
        /// <param name="tilePositions"> The tilemap positions corresponding to the tiles to alter in the tilemap. </param>
        /// <param name="soundsController"> The controller used to play all trap-related SFX. </param>
        public virtual void Construct(Vector3 spawnPosition, Canvas canvas, 
            Vector3Int[] tilePositions, TrapSoundsController soundsController) {
            // Initialize all the bookkeeping structures we will need
            SpawnPosition = spawnPosition;
            TilePositions = tilePositions;
            SoundsController = soundsController;
            
            // Delete/invalidate all the tiles overlapping the trap
            TilemapManager.Instance.ClearLevelTiles(TilePositions);

            // Spawn a slider to indicate the progress on the build
            GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
            sliderObject.transform.position = spawnPosition + AnimationOffset + Vector3.up;
            BuildCompletionBar = sliderObject.GetComponent<Slider>();

            // Trap deployment visuals
            // Slide down trap and fade it into existence
            transform.position = spawnPosition + Vector3.up * 3f;
            transform.DOMove(spawnPosition + Vector3.up * AnimationOffset.y, 0.2f);

            var sprite = GetComponent<SpriteRenderer>();
            var color = sprite.color;
            sprite.color = new Color(color.r, color.g, color.b, 0);
            sprite.DOFade(1, 0.4f);

            // Initiate the build time countdown
            ConstructionCompletion = 0;
        }
        
        /// <summary>
        /// Adjusts the trap spawn position relative to an origin position when deploying a trap to the left of
        /// the player, facing the negative direction along the x-axis.
        /// </summary>
        /// <param name="origin"> The central position for the trap to be spawned. </param>
        /// <returns> The resulting trap spawn position after the application of offset adjustments. </returns>
        public Vector3 GetLeftSpawnPoint(Vector3 origin) {
            return origin + LeftSpawnOffset;
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
        public List<Vector3Int> GetLeftGridPoints() {
            return LeftGridPoints;
        }

        /// <summary>
        /// Retrieves the tilemap grid offset points for trap deployment when the player faces the positive
        /// direction along the x-axis.
        /// </summary>
        /// <returns> A list of tilemap offsets to pinpoint tiles needed for trap deployment. </returns>
        /// <remarks> Depending on the shape and extent of the trap, the offsets will vary between trap types. </remarks>
        public List<Vector3Int> GetRightGridPoints() {
            return RightGridPoints;
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
            return score >= ValidationScore;
        }
        
        #endregion
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        #region Trap Building
        
        /// <summary>
        /// Applies an animation to the trap while it's being built.
        /// </summary>
        /// <remarks> Invoked every frame that the trap is being built. </remarks>
        protected virtual void BuildTrap() {
            // Randomly shake the trap along the x and y-axis
            Vector3 targetPosition = new Vector3(SpawnPosition.x + Random.Range(-0.5f, 0.5f),
                SpawnPosition.y + Random.Range(0.01f, 0.2f));

            // Shake out then back
            transform.DOMove(targetPosition, 0.05f);
            transform.DOMove(SpawnPosition + Vector3.up * AnimationOffset.y, 0.05f);
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
        /// or unleashed on a <see cref="Hero"/>. 
        /// </summary>
        protected abstract void DetonateTrap();

        /// <summary>
        /// Cleans up the trap data and frees the used trap tiles upon the completion of the trap detonation animation.
        /// </summary>
        protected virtual void OnDetonateTrapAnimationCompete() {}
        
        /// <summary>
        /// Applies resulting effects and reactions to the <see cref="Hero"/> upon trap detonation. 
        /// </summary>
        /// <remarks> Invoked upon triggered collision with a <see cref="Hero"/>. </remarks>
        protected abstract void OnEnteredTrap(IDamageable actor);
        
        /// <summary>
        /// Applies lasting effects and reactions to the <see cref="Hero"/> when exiting the trap collider. 
        /// </summary>
        /// <remarks> Invoked upon exiting a trigger collision with a <see cref="Hero"/>. </remarks>
        protected abstract void OnExitedTrap(IDamageable actor);
        
        #endregion
        
        #endregion
    }
}
