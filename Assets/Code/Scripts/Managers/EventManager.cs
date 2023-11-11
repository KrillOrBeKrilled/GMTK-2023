namespace KrillOrBeKrilled.Managers {
  public class EventManager {
    public static EventManager Instance => _instance ??= new EventManager();
    private static EventManager _instance;

    // Game Manager Events
    public readonly GameOverEvent GameOverEvent;

    // Coins & Resources
    public readonly CoinAmountChangedEvent CoinAmountChangedEvent;


    // UI Events

    private EventManager() {
      // Game Manager Events
      this.GameOverEvent = new GameOverEvent();

      // Coins & Resources
      this.CoinAmountChangedEvent = new CoinAmountChangedEvent();

      // UI Events
    }
  }
}
