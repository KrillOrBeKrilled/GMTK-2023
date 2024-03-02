using System.Collections;
using DG.Tweening;
using KrillOrBeKrilled.Traps.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// AcidPitTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="InGroundTrap"/> that fills a permanent 2x2 grounded
    /// square of liquid into the ground and damages the hero in intervals with a
    /// speed penalty. 
    /// </summary>
    /// <remarks> The default building animations are overrided with shader support
    /// to fill the pit with liquid and heat haze effects. </remarks>
    public class AcidPitTrap : InGroundTrap {
        // --------------- Animations ----------------
        [Tooltip("Holds the reference to the acid liquid material to be adjusted during trap building process.")]
        [SerializeField] private MeshRenderer _acidLiquid;
        [Tooltip("Holds the reference to the heat haze material to be adjusted during trap building process.")]
        [SerializeField] private SpriteRenderer _heatEmanating;
        [Tooltip("Holds the reference to the animated bubbles GameObject to be adjusted during trap building process.")]
        [SerializeField] private GameObject _bubbles;
        [Tooltip("The length of time required to build the trap when the player stands idle and in range.")]
        [SerializeField] protected float BuildingDuration;
        private Vector3 _bubblesStartPos;
        
        // -------------- Hero Effects ---------------
        [Tooltip("Interval damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;

        private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
        private Coroutine _intervalDamageCoroutine;
        
        private readonly int _acidDepthKey = Shader.PropertyToID("_Depth");
        private readonly int _distortionRangeKey = Shader.PropertyToID("_HazeRange");
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region Trap Deployment
        
        /// <inheritdoc cref="Trap.Construct"/>
        /// <remarks> Extended to change the shader animations for the acid liquid. </remarks>
        public override void Construct(Vector3 spawnPosition, Canvas canvas, 
            TrapSoundsController soundsController, UnityAction onDestroy = null) {
            // Initialize all the bookkeeping structures we will need
            SpawnPosition = spawnPosition;
            SoundsController = soundsController;
            
            // TODO: Reuse later for durability
            // GameObject sliderObject = Instantiate(this.SliderBar, canvas.transform);
            // sliderObject.transform.position = spawnPosition + this.AnimationOffset + Vector3.up;
            // this.BuildCompletionBar = sliderObject.GetComponent<Slider>();

            // Trap deployment visuals
            this.transform.position = spawnPosition;
            this._bubblesStartPos = this._bubbles.transform.position;

            // Set the acid liquid level and heat haze intensity to 0
            this._acidLiquid.material.SetFloat(this._acidDepthKey, 0);
            this._heatEmanating.material.SetFloat(this._distortionRangeKey, 0);
            this._bubbles.SetActive(false);
            
            // Make construction animations
            this.BuildTrap();
            this.SoundsController.OnBuildComplete();
            this.SetUpTrap();
        }

        #endregion
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods
        
        #region Trap Detonation
        
        /// <inheritdoc cref="Trap.DetonateTrap"/>
        /// <remarks> This trap cannot be detonated and forever stays dug into the ground with moving liquid. </remarks>
        protected override void DetonateTrap() {}

        /// <inheritdoc cref="Trap.OnEnteredTrap"/>
        /// <summary>
        /// This trap applies an 80% speed reduction and interval damage to the <see cref="ITrapDamageable"/> actor
        /// every second it remains in the acid.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnEnteredTrap(ITrapDamageable actor) {
            // Make the hero reflexively leap out of the burning acid pit
            actor.ThrowActorForward(1f);
            actor.ApplySpeedPenalty(0.8f);

            if (this._intervalDamageCoroutine != null) {
                this.StopCoroutine(this._intervalDamageCoroutine);
            }
            this._intervalDamageCoroutine = this.StartCoroutine(this.DealIntervalDamage(actor));
        }

        /// <inheritdoc cref="Trap.OnExitedTrap"/>
        /// <summary>
        /// Resets the speed reduction and stops dealing damage to the <see cref="ITrapDamageable"/> actor.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        protected override void OnExitedTrap(ITrapDamageable actor) {
            actor.ResetSpeedPenalty();

            if (this._intervalDamageCoroutine != null)
                this.StopCoroutine(this._intervalDamageCoroutine);
        }

        #endregion
        
        #region Trap Building

        /// <inheritdoc cref="Trap.BuildTrap"/>
        /// <summary>
        /// Overridden to create a new build animation to fill the pit with acid and heat haze, while adjusting the
        /// position of the rising bubbles.
        /// </summary>
        protected override void BuildTrap() {
            // Clamp the acid depth to prevent the acid from looking strange around tile edges
            DOVirtual.Float(0f, 0.8f, this.BuildingDuration, targetDepth => {
                // Magic ratio to avoid making the haze too intense
                var targetHeatHazeRange = Mathf.Clamp(targetDepth / 3.8f, 0, 1);

                this._acidLiquid.material.SetFloat(this._acidDepthKey, targetDepth);
                this._heatEmanating.material.SetFloat(this._distortionRangeKey, targetHeatHazeRange);

                if (!this._bubbles.activeSelf && targetDepth > 0.05f) {
                    this._bubbles.SetActive(true);
                }

                this._bubbles.transform.position = new Vector3(this._bubblesStartPos.x,
                                                               this._bubblesStartPos.y + targetDepth * 1.8f,
                                                               this._bubblesStartPos.z);
            }).SetEase(Ease.InOutCubic);
        }
        
        protected override void SetUpTrap() {}
        
        #endregion
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Deals damage to the <see cref="ITrapDamageable"/> actor each second for as long as its health remains
        /// greater than zero.
        /// </summary>
        /// <param name="actor"> The recipient of the trap's damaging effects. </param>
        /// <remarks> The coroutine is started and stopped by <see cref="OnEnteredTrap"/>. </remarks>
        private IEnumerator DealIntervalDamage(ITrapDamageable actor) {
            while (actor.GetHealth() > 0) {
                actor.TakeDamage(this._damageAmount, this);
                yield return this._waitForOneSecond;
            }
            
            this._intervalDamageCoroutine = null;
        }
        
        #endregion
    }
}
