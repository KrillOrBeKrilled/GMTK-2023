using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Common {
  public class GameEvent<T> : ScriptableObject {
    private readonly List<GameEventListener<T>> eventListeners = new();

    public void Raise(T value) {
      for (int i = this.eventListeners.Count - 1; i >= 0; i--)
        this.eventListeners[i].OnEventRaised(value);
    }

    public void RegisterListener(GameEventListener<T> listener) {
      if (!eventListeners.Contains(listener))
        eventListeners.Add(listener);
    }
    
    public void UnregisterListener(GameEventListener<T> listener) {
      if (eventListeners.Contains(listener))
        eventListeners.Remove(listener);
    }
  }
}