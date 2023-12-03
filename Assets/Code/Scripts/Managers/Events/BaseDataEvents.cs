using KrillOrBeKrilled.Traps;
using UnityEngine.Events;

namespace KrillOrBeKrilled.Managers {
    //*******************************************************************************************
    // CoinAmountChangedEvent
    //*******************************************************************************************
    /// <summary>
    /// A callback that tracks when the coin quantity of the coin management system has
    /// been updated.
    /// </summary>
    public class CoinAmountChangedEvent : UnityEvent<int> {}
    
    //*******************************************************************************************
    // ResourceAmountChangedEvent
    //*******************************************************************************************
    /// <summary>
    /// A callback that tracks when the resource inventory has been updated.
    /// </summary>
    public class ResourceAmountChangedEvent : UnityEvent<ResourceType, int> {}
    
    //*******************************************************************************************
    // PauseToggledEvent
    //*******************************************************************************************
    /// <summary>
    /// A callback that tracks when the game should be paused or unpaused.
    /// </summary>
    public class PauseToggledEvent : UnityEvent<bool> {}
}
