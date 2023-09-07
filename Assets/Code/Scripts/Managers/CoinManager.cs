using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//*******************************************************************************************
// CoinManager
//*******************************************************************************************
namespace Managers {
    /// <summary>
    /// Manages the coin count through earning and consumption throughout gameplay.
    /// </summary>
    /// <remarks> Exposes methods to check coin affordability, and earn and consume
    /// coins. </remarks>
    public class CoinManager : Singleton<CoinManager> {
        [Tooltip("Repeated time interval to wait to earn coins.")]
        [SerializeField] private float _earnCoinInterval = 5f;

        [Tooltip("The current amount of coins possessed to build traps.")]
        public int Coins { get; private set; }
        
        [Tooltip("Tracks when the coin amount changes.")]
        public UnityEvent<int> OnCoinAmountChanged;

        private WaitForSeconds _waitInterval;

        /// <summary>
        /// Checks the current number of <see cref="Coins"/> against a cost and evaluates affordability.
        /// </summary>
        /// <param name="cost"> The required amount of coins. </param>
        /// <returns> If the current number of <see cref="Coins"/> is enough to afford the given cost. </returns>
        public bool CanAfford(int cost) {
            return this.Coins >= cost;
        }

        /// <summary>
        /// Increments the current number of coins.
        /// </summary>
        /// <param name="amount"> The number of coins to increase the amount of <see cref="Coins"/>. </param>
        /// <remarks> Invokes the <see cref="OnCoinAmountChanged"/> event. </remarks>
        public void EarnCoins(int amount) {
            this.Coins += amount;
            this.OnCoinAmountChanged?.Invoke(this.Coins);
        }

        /// <summary>
        /// Decrements the current number of coins.
        /// </summary>
        /// <param name="amount"> The number of coins to decrease the amount of <see cref="Coins"/>. </param>
        /// <remarks> Invokes the <see cref="OnCoinAmountChanged"/> event. This method should only be
        /// used if <see cref="CanAfford"/> passes. </remarks>
        public void ConsumeCoins(int amount) {
            this.Coins -= amount;
            this.OnCoinAmountChanged?.Invoke(this.Coins);
        }
        
        /// <summary>
        /// Begins the coroutine to earn coins.
        /// </summary>
        public void StartCoinEarning() {
            this.StartCoroutine(this.EarnCoinCoroutine());
        }

        protected override void Awake() {
            base.Awake();
            this.OnCoinAmountChanged = new UnityEvent<int>();
            this._waitInterval = new WaitForSeconds(this._earnCoinInterval);
        }

        /// <remarks> Invokes the <see cref="OnCoinAmountChanged"/> event. </remarks>
        private void Start() {
            this.Coins = 1;
            this.OnCoinAmountChanged?.Invoke(1);
        }

        /// <summary>
        /// Indefinitely increments the <see cref="Coins"/> between
        /// <see cref="_earnCoinInterval">_earnCoinIntervals</see> of time.
        /// </summary>
        /// <remarks> The coroutine is started by <see cref="StartCoinEarning"/>. </remarks>
        private IEnumerator EarnCoinCoroutine() {
            while (true) {
                yield return this._waitInterval;
                this.EarnCoins(1);
            }
        }
    }
}
