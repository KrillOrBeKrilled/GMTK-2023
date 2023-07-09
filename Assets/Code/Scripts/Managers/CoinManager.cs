using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : Singleton<CoinManager> {
  [SerializeField] private float _earnCoinInterval = 5f;

  public int Coins { get; private set; }
  public UnityEvent<int> OnCoinAmountChanged;

  private WaitForSeconds _waitInterval;

  public bool CanAfford(int cost) {
    return this.Coins >= cost;
  }

  public void EarnCoins(int amount) {
    this.Coins += amount;
    this.OnCoinAmountChanged?.Invoke(this.Coins);
  }

  public void ConsumeCoins(int amount) {
    this.Coins -= amount;
    this.OnCoinAmountChanged?.Invoke(this.Coins);
  }

  protected override void Awake() {
    base.Awake();
    this.OnCoinAmountChanged = new UnityEvent<int>();
    this._waitInterval = new WaitForSeconds(this._earnCoinInterval);
  }

  private void Start() {
    this.StartCoroutine(this.EarnCoinCoroutine());
    this.Coins = 1;
    this.OnCoinAmountChanged?.Invoke(1);
  }

  private IEnumerator EarnCoinCoroutine() {
    while (true) {
      yield return this._waitInterval;
      this.EarnCoins(1);
    }
  }
}
