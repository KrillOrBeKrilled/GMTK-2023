using KrillOrBeKrilled.Common;
using KrillOrBeKrilled.Managers;
using KrillOrBeKrilled.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// Hero
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes {
    /// <summary>
    /// Manages the states of the hero throughout narrative sequences and gameplay,
    /// tracking health and playing associated SFX. Manipulates movement in the
    /// narrative sequences through <see cref="HeroMovement"/>.
    /// </summary>
    public class Hero : MonoBehaviour, IDamageable {
        public HeroMovement HeroMovement => this._heroMovement;
        private HeroMovement _heroMovement;

        private Animator _animator;
        private const int CoinsEarnedOnDeath = 2;

        // ---------------- Spawning -----------------
        // [Tooltip("Determines the position to respawn the hero.")]
        // public HeroRespawnPoint RespawnPoint;

        [Tooltip("Duration of time for the hero to recover at the RespawnPoint before moving again.")]
        [SerializeField] internal float RespawnTime = 3;

        private static readonly int SpawningKey = Animator.StringToHash("spawning");

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

        [Tooltip("Tracks when the hero has run out of lives.")]
        public UnityEvent OnGameOver;

        [Tooltip("Tracks when the hero initialization is complete.")]
        public UnityEvent OnHeroReset;

        private void Awake() {
            this.TryGetComponent(out this._heroMovement);
            this.TryGetComponent(out this._animator);
            this.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);
        }

        public void Initialize(HeroData heroData, HeroSoundsController soundsController) {
            this.Health = heroData.Health;
            this.Type = heroData.Type;
            this._soundsController = soundsController;

            this._heroMovement.Initialize(soundsController);
        }

        //========================================
        // Level Dialogue Sequence
        //========================================

#region Level Dialogue Sequence
        /// <summary> Enables movement until the hero enters the level. </summary>
        public void EnterLevel() {
            this.StartCoroutine(this.EnterLevelAnimation());
        }

        /// <summary>
        /// Enables movement through <see cref="HeroMovement"/> for a duration of time and then disables
        /// movement.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="EnterLevel"/>. </remarks>
        private IEnumerator EnterLevelAnimation() {
            this._heroMovement.ToggleMoving(true);

            yield return new WaitForSeconds(2f);

            this._heroMovement.ToggleMoving(false);
        }

        /// <summary> Enables movement through <see cref="HeroMovement"/>. </summary>
        public void StartRunning() {
            this.StopAllCoroutines();
            this._heroMovement.ToggleMoving(true);
        }
#endregion

        //========================================
        // IDamageable Implementations
        //========================================

#region IDamageable Implementations
        public int GetHealth() {
            return this.Health;
        }
        
        /// <summary>
        /// Decrements the hero's health, earns coins through the <see cref="CoinManager"/>,
        /// and plays associated SFX. If the health falls to zero or below, triggers the hero death.
        /// </summary>
        /// <param name="amount"> The value to subtract from the hero's health. </param>
        /// <remarks> Invokes the <see cref="OnHealthChanged"/> event. </remarks>
        public void TakeDamage(int amount) {
            this.Health -= amount;

            _soundsController.OnTakeDamage();

            this.OnHealthChanged?.Invoke(this.Health);

            if (this.Health <= 0) {
                this.Die();
            }
        }

        /// <summary>
        /// Decrements the hero's number of lives, playing associated SFX. If the number of lives are
        /// reduced to zero, broadcasts the endgame and destroys this GameObject. Otherwise, respawns
        /// the hero.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnHeroDied"/> event. If the number of lives are reduced to
        /// zero, invokes the <see cref="OnGameOver"/> event. </remarks>
        public void Die() {
            _soundsController.OnHeroDeath();

            CoinManager.Instance.EarnCoins(CoinsEarnedOnDeath);

            this.OnHeroDied?.Invoke(this);
            Destroy(this.gameObject);
        }

        public void ThrowActorBack(float stunDuration, float throwForce) {
            this._heroMovement.ThrowHeroBack(stunDuration, throwForce);
        }

        public void ApplySpeedPenalty(float penalty) {
            this._heroMovement.SetSpeedPenalty(penalty);
        }

        public void ResetSpeedPenalty() {
            this._heroMovement.ResetSpeedPenalty();
        }
#endregion

        //========================================
        // Gameplay Loop
        //========================================
        
#region Gameplay Loop
        /// <summary>
        /// Triggers hero death through <see cref="Die"/>.
        /// </summary>
        /// <remarks> Subscribed to the <see cref="HeroMovement.OnHeroIsStuck"/> event. </remarks>
        private void OnHeroIsStuck(float xPos, float yPos, float zPos) {
            this.Die();
        }
#endregion
    }
}
