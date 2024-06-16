using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Common {
  [CreateAssetMenu(menuName = "Events/GameEvent", fileName = "GameEvent")]  
  public class GameEvent : ScriptableObject {
    private readonly List<GameEventListener> eventListeners = new();

    public void Raise() {
      for (int i = this.eventListeners.Count - 1; i >= 0; i--)
        this.eventListeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener) {
      if (!eventListeners.Contains(listener))
        eventListeners.Add(listener);
    }
    
    public void UnregisterListener(GameEventListener listener) {
      if (eventListeners.Contains(listener))
        eventListeners.Remove(listener);
    }
  }
}