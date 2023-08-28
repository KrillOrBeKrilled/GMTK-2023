using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// Hero
//*******************************************************************************************
namespace Heroes {
    /// <summary>
    /// Manages the states of the hero throughout narrative sequences and gameplay,
    /// tracking health and playing associated SFX. Manipulates movement in the
    /// narrative sequences through <see cref="HeroMovement"/>.
    /// </summary>
    public class Hero : MonoBehaviour {
        public HeroMovement HeroMovement => this._heroMovement;
        private HeroMovement _heroMovement;
        
        private Animator _animator;
        
        // ---------------- Spawning -----------------
        [Tooltip("Determines the position to respawn the hero.")]
        public HeroRespawnPoint RespawnPoint;
        [Tooltip("Duration of time for the hero to recover at the RespawnPoint before moving again.")]
        public float RespawnTime = 3;
        private static readonly int SpawningKey = Animator.StringToHash("spawning");
      
        // ----------------- Health ------------------
        [Tooltip("The current health of the hero.")]
        public int Health { get; private set; }
        [Tooltip("The current number of lives the hero has left.")]
        public int Lives { get; private set; }

        [Tooltip("The maximum health value used upon initialization and respawns.")]
        public const int MaxHealth = 100;
        [Tooltip("The maximum number of times the hero can die before game over.")]
        public const int MaxLives = 3;
        
        // ------------------ SFX --------------------
        public AK.Wwise.Event HeroHurtEvent;

        // ----------------- Events ------------------
        [Tooltip("Tracks when the hero's health changes.")]
        public UnityEvent<int> OnHealthChanged;
        [Tooltip("Tracks when the hero dies.")]
        public UnityEvent<int, float, float, float> OnHeroDied;
        [Tooltip("Tracks when the hero has run out of lives.")]
        public UnityEvent OnGameOver;
        [Tooltip("Tracks when the hero initialization is complete.")]
        public UnityEvent OnHeroReset;

        private void Awake() {
            this.TryGetComponent(out this._heroMovement);
            this.TryGetComponent(out this._animator);
            this.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

            this.ResetHero();
        }
        
        /// <summary>
        /// Disables movement and fills the health and lives to their maximum values specified by
        /// <see cref="MaxHealth"/> and <see cref="MaxLives"/>.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnHealthChanged"/> and <see cref="OnHeroReset"/> events. </remarks>
        public void ResetHero()
        {
            this._heroMovement.ToggleMoving(false);
            this.Health = MaxHealth;
            this.Lives = MaxLives;
            this.OnHealthChanged.Invoke(this.Health);
            this.OnHeroReset.Invoke();
        }
        
        //========================================
        // Setters
        //========================================
        
        /// <summary>
        /// Sets the <see cref="RespawnPoint"/> to the provided <see cref="HeroRespawnPoint"/>.
        /// </summary>
        /// <param name="respawnPoint"> The new respawn point for the hero to respawn to upon death. </param>
        public void SetRespawnPoint(HeroRespawnPoint respawnPoint)
        {
            this.RespawnPoint = respawnPoint;
        }
        
        //========================================
        // Level Dialogue Sequence
        //========================================
        
        /// <summary>
        /// Enables movement until the hero enters the level.
        /// </summary>
        public void EnterLevel()
        {
            this.StartCoroutine(this.EnterLevelAnimation());
        }

        /// <summary>
        /// Enables movement through <see cref="HeroMovement"/> for a duration of time and then disables
        /// movement.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="EnterLevel"/>. </remarks>
        private IEnumerator EnterLevelAnimation()
        {
            this._heroMovement.ToggleMoving(true);

            yield return new WaitForSeconds(2f);

            this._heroMovement.ToggleMoving(false);
        }
        
        /// <summary> Enables movement through <see cref="HeroMovement"/>. </summary>
        public void StartRunning()
        {
            this.StopAllCoroutines();
            this._heroMovement.ToggleMoving(true);
        }
        
        //========================================
        // Gameplay Loop
        //========================================
        
        /// <summary>
        /// Decrements the hero's health by the provided amount, earns coins through the <see cref="CoinManager"/>,
        /// and plays associated SFX. If the health falls to zero or below, triggers the hero death.
        /// </summary>
        /// <param name="amount"> The value to subtract from the hero's health. </param>
        /// <remarks> Invokes the <see cref="OnHealthChanged"/> event. </remarks>
        public void TakeDamage(int amount) {
            this.Health -= amount;
            CoinManager.Instance.EarnCoins(1);

            if (this.Health <= 0) {
                this.Die();
            }

            this.HeroHurtEvent.Post(this.gameObject);
            this.OnHealthChanged?.Invoke(this.Health);
        }

        /// <summary>
        /// Decrements the hero's number of lives, playing associated SFX. If the number of lives are
        /// reduced to zero, broadcasts the endgame and destroys this GameObject. Otherwise, respawns
        /// the hero.
        /// </summary>
        /// <remarks> Invokes the <see cref="OnHeroDied"/> event. If the number of lives are reduced to
        /// zero, invokes the <see cref="OnGameOver"/> event. </remarks>
        public void Die()
        {
            this.Lives--;
            this.HeroHurtEvent.Post(this.gameObject);

            var heroPos = this.transform.position;
            this.OnHeroDied?.Invoke(this.Lives, heroPos.x, heroPos.y, heroPos.z);

            if (this.Lives == 0)
            {
                this.OnGameOver.Invoke();
                Destroy(this.gameObject);
                return;
            }

            this.StartCoroutine(this.Respawn());
        }

        /// <summary>
        /// Triggers hero death through <see cref="Die"/>.
        /// </summary>
        /// <remarks> Listens on the <see cref="HeroMovement.OnHeroIsStuck"/> event. </remarks>
        private void OnHeroIsStuck(float xPos, float yPos, float zPos)
        {
            this.Die();
        }

        /// <summary>
        /// Freezes the hero's position to the <see cref="RespawnPoint"/> <see cref="Transform"/> position,
        /// triggering associated animations, and gradually refills the health bar for
        /// <see cref="RespawnTime"/> duration. Once the health bar refills, enables movement and movement
        /// animations.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="Die"/> if the hero has any remaining lives.
        /// Invokes the <see cref="OnHealthChanged"/> event for each cycle that the health bar refills. </remarks>
        private IEnumerator Respawn()
        {
            this.transform.position = this.RespawnPoint.transform.position;
            this._animator.SetBool(SpawningKey, true);
            this._heroMovement.ToggleMoving(false);

            // Gradually fill the health bar over the respawn time
            float timePassed = 0;
            while (timePassed < this.RespawnTime)
            {
                timePassed += Time.deltaTime;

                this.Health = (int) Mathf.Lerp(0f, MaxHealth, timePassed / this.RespawnTime);
                this.OnHealthChanged.Invoke(this.Health);

                yield return new WaitForEndOfFrame();
            }

            this._heroMovement.ToggleMoving(true);
            this._animator.SetBool(SpawningKey, false);
        }
    }
}
