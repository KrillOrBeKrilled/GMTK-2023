using System.Collections;
using KrillOrBeKrilled.Common.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// AcidPitTrap
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// A subclass of <see cref="InGroundTrap"/> that fills a permanent 2x2 grounded
    /// square of liquid into the ground and damages the <see cref="IDamageable"/> in
    /// intervals with a speed penalty. 
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
        private Vector3 _bubblesStartPos;
        
        // -------------- Hero Effects ---------------
        [Tooltip("Interval damage to be applied to the Hero upon collision.")]
        [SerializeField] private int _damageAmount;
        [Tooltip("The HeroJumpPad used to allow the hero to bypass the pit before the building process is completed.")]
        [SerializeField] private GameObject _pitAvoidanceJumpPad;

        private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
        private Coroutine _intervalDamageCoroutine;
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region Trap Deployment
        
        /// <inheritdoc cref="Trap.Construct"/>
        /// <remarks> Extended to change the shader animations for the acid liquid. </remarks>
        public override void Construct(Vector3 spawnPosition, Canvas canvas, 
            Vector3Int[] tilePositions, TrapSoundsController soundsController, UnityAction onDestroy = null) {
            // Initialize all the bookkeeping structures we will need
            SpawnPosition = spawnPosition;
            TilePositions = tilePositions;
            SoundsController = soundsController;
            
            // Spawn a slider to indicate the progress on the build
            GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
            sliderObject.transform.position = spawnPosition + AnimationOffset + Vector3.up;
            BuildCompletionBar = sliderObject.GetComponent<Slider>();

            // Trap deployment visuals
            transform.position = spawnPosition;
            _bubblesStartPos = _bubbles.transform.position;

            // Set the acid liquid level and heat haze intensity to 0
            _acidLiquid.material.SetFloat("_Depth", 0);
            _heatEmanating.material.SetFloat("_HazeRange", 0);
            _bubbles.SetActive(false);

            // Initiate the build time countdown
            ConstructionCompletion = 0;
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
        /// This trap applies an 80% speed reduction to the <see cref="HeroMovement"/> and interval damage to the
        /// <see cref="Hero"/> every second the <see cref="Hero"/> remains in the acid.
        /// </summary>
        protected override void OnEnteredTrap(IDamageable actor) {
            if (!this.IsReady) 
                return;

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
        /// Resets the speed reduction through <see cref="HeroMovement"/> and stops dealing damage to the
        /// <see cref="Hero"/>.
        /// </summary>
        protected override void OnExitedTrap(IDamageable actor) {
            if (!this.IsReady) 
                return;

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
            var targetDepth = Mathf.Clamp(ConstructionCompletion, 0, 0.8f);

            // Magic ratio to avoid making the haze too intense
            var targetHeatHazeRange = Mathf.Clamp(targetDepth / 3.8f, 0, 1);

            _acidLiquid.material.SetFloat("_Depth", targetDepth);
            _heatEmanating.material.SetFloat("_HazeRange", targetHeatHazeRange);

            if (!_bubbles.activeSelf && targetDepth > 0.05f) {
                _bubbles.SetActive(true);
            }

            _bubbles.transform.position = new Vector3(_bubblesStartPos.x, _bubblesStartPos.y + targetDepth * 1.8f,
                _bubblesStartPos.z);
        }
        
        protected override void SetUpTrap() {}
        
        #endregion
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Deals damage to the <see cref="IDamageable"/> each second for as long as its health remains
        /// greater than zero.
        /// </summary>
        /// <param name="hero"> The hero receiving damage. </param>
        /// <remarks> The coroutine is started and stopped by <see cref="OnEnteredTrap"/>. </remarks>
        private IEnumerator DealIntervalDamage(IDamageable actor) {
            while (actor.GetHealth() > 0) {
                actor.TakeDamage(this._damageAmount);
                yield return this._waitForOneSecond;
            }
            
            this._intervalDamageCoroutine = null;
        }
        
        #endregion
    }
}
