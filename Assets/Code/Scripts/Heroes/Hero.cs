using KrillOrBeKrilled.Common.Interfaces;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Model;
using KrillOrBeKrilled.Heroes.AI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

//*******************************************************************************************
// Hero
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes {
    /// <summary>
    /// Manages the states of the hero throughout narrative sequences and gameplay,
    /// tracking health and playing associated SFX. Manipulates movement in the
    /// narrative sequences through the <see cref="HeroBT"/>.
    /// </summary>
    public class Hero : MonoBehaviour, IDamageable {
        private Rigidbody2D _rigidbody;
        private HeroBT _heroBrain;
        private FieldOfView _heroSight;
        
        private const int CoinsEarnedOnDeath = 2;

        // ----------------- Health ------------------
        [Tooltip("The current health of the hero.")]
        public int Health { get; private set; }

        // ----------------- Data --------------------
        public HeroData.HeroType Type { get; private set; }
        
        // ------------- Sound Effects ---------------
        private HeroSoundsController _soundsController;

        // ----------------- Events ------------------
        [Tooltip("Tracks when the hero's health changes.")]
        public UnityEvent<int> OnHealthChanged;

        [Tooltip("Tracks when the hero dies.")]
        public UnityEvent<Hero> OnHeroDied;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this.TryGetComponent(out this._rigidbody);
            this.TryGetComponent(out this._heroBrain);
            this.TryGetComponent(out this._heroSight);
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        #region IDamageable Implementations
        
        /// <summary>
        /// Reduces the hero's movement speed by a percentage reduction via the <see cref="HeroBT"/>.
        /// </summary>
        /// <param name="penalty"> The speed penalty value to limit the hero's movement. </param>
        /// <remarks> The incremented speed penalty is clamped between [0,1] as a percentage value. </remarks>
        public void ApplySpeedPenalty(float penalty) {
            this._heroBrain.UpdateData("SpeedPenalty", Mathf.Clamp(penalty, 0f, 1f));
        }
        
        /// <summary>
        /// Decrements the hero's number of lives, playing associated SFX. If the number of lives are
        /// reduced to zero, broadcasts the endgame and destroys this GameObject. Otherwise, respawns
        /// the hero.
        /// </summary>
        /// <remarks>
        /// Invokes the <see cref="OnHeroDied"/> event.
        /// </remarks>
        public void Die() {
            this._soundsController.OnHeroDeath();
            CoinManager.Instance.EarnCoins(CoinsEarnedOnDeath);

            this.OnHeroDied?.Invoke(this);
            Destroy(this.gameObject);
        }
        
        public int GetHealth() {
            return this.Health;
        }
        
        /// <summary>
        /// Resets the speed penalty to return the hero movement speed to normal via the <see cref="HeroBT"/>.
        /// </summary>
        public void ResetSpeedPenalty() {
            this._heroBrain.UpdateData("SpeedPenalty", 0f);
        }
        
        /// <summary>
        /// Decrements the hero's health, earns coins through the <see cref="CoinManager"/>,
        /// and plays associated SFX. If the health falls to zero or below, triggers the hero death.
        /// </summary>
        /// <param name="amount"> The value to subtract from the hero's health. </param>
        /// <remarks> Invokes the <see cref="OnHealthChanged"/> event. </remarks>
        public void TakeDamage(int amount) {
            this.Health -= amount;
            this._soundsController.OnTakeDamage();
            this.OnHealthChanged?.Invoke(this.Health);

            if (this.Health <= 0) {
                this.Die();
            }
        }

        /// <summary>
        /// Applies a force to knock back and stun the hero via the <see cref="HeroBT"/>.
        /// </summary>
        /// <param name="stunDuration"> The duration of time to stun the hero. </param>
        /// <param name="throwForce"> Scales the knock back force applied to the hero. </param>
        public void ThrowActorBack(float stunDuration, float throwForce) {
            this._heroBrain.UpdateData("IsStunned", true);
            this._heroBrain.UpdateData("StunDuration", stunDuration);
            
            Vector2 explosionVector = new Vector2(-1f, 0.7f) * throwForce;
            this._rigidbody.AddForce(explosionVector, ForceMode2D.Impulse);
        }
        
        public void ThrowActorForward(float throwForce) {
            this._rigidbody.velocity = Vector2.zero;
            Vector2 leapVector = new Vector2(0.18f, 2f) * throwForce;
            this._rigidbody.AddForce(leapVector, ForceMode2D.Impulse);
        }

        #endregion
        
        /// <summary>
        /// Enables movement until the hero enters the level.
        /// </summary>
        public void EnterLevel() {
            this.StartCoroutine(this.EnterLevelAnimation());
        }
        
        public void Initialize(HeroData heroData, HeroSoundsController soundsController, Tilemap groundTilemap) {
            this.Health = heroData.Health;
            this.Type = heroData.Type;
            this._soundsController = soundsController;

            this._heroBrain.Initialize(soundsController);
            this._heroSight.Initialize(groundTilemap);
        }
        
        /// <summary>
        /// Enables movement through the <see cref="HeroBT"/>.
        /// </summary>
        public void StartRunning() {
            this.StopAllCoroutines();
            this._heroBrain.UpdateData("IsMoving", true);
        }
        
        /// <summary>
        /// Disables movement through the <see cref="HeroBT"/>.
        /// </summary>
        public void StopRunning() {
            this._heroBrain.UpdateData("IsMoving", false);
        }
        
        #endregion

        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Enables movement through the <see cref="HeroBT"/> for a duration of time and then disables
        /// movement.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="EnterLevel"/>. </remarks>
        private IEnumerator EnterLevelAnimation() {
            this._heroBrain.UpdateData("IsMoving", true);

            yield return new WaitForSeconds(2f);
            
            this._heroBrain.UpdateData("IsMoving", false);
        }
        
        #endregion
    }
}
