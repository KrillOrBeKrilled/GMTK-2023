using UnityEngine.Events;

namespace KrillOrBeKrilled.Managers {
  public class CoinAmountChangedEvent : UnityEvent<int> {}
  public class PauseToggledEvent : UnityEvent<bool> {}
}
