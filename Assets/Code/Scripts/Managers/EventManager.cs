//*******************************************************************************************
// EventManager
//*******************************************************************************************
namespace KrillOrBeKrilled.Managers {
    /// <summary>
    /// Exposes events principal to the lifecycle of the system on a global scale.
    /// </summary>
    public class EventManager {
        public static EventManager Instance => _instance ??= new EventManager();
        private static EventManager _instance;

        // Game Events
        public readonly GameOverEvent GameOverEvent;
        public readonly PauseToggledEvent PauseToggledEvent;
        
        // Coins & Resources
        public readonly CoinAmountChangedEvent CoinAmountChangedEvent;
        public readonly ResourceAmountChangedEvent ResourceAmountChangedEvent;

        // UI Events
        public readonly ShowDialogueUIEvent ShowDialogueUIEvent;
        public readonly HideDialogueUIEvent HideDialogueUIEvent;

        private EventManager() {
            // Game Events
            this.GameOverEvent = new GameOverEvent();
            this.PauseToggledEvent = new PauseToggledEvent();
            
            // Coins & Resources
            this.CoinAmountChangedEvent = new CoinAmountChangedEvent();
            this.ResourceAmountChangedEvent = new ResourceAmountChangedEvent();
            
            // UI Events
            this.ShowDialogueUIEvent = new ShowDialogueUIEvent();
            this.HideDialogueUIEvent = new HideDialogueUIEvent();
        }
    }
}
