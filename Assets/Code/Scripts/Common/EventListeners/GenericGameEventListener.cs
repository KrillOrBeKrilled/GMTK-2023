using UnityEngine;
using UnityEngine.Events;

namespace KrillOrBeKrilled.Common {
  public class GameEventListener<T> : MonoBehaviour
  {
    [Tooltip("Event to register with.")]
    public GameEvent<T> Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<T> Response;

    private void OnEnable()
    {
      Event.RegisterListener(this);
    }

    private void OnDisable()
    {
      Event.UnregisterListener(this);
    }

    public virtual void OnEventRaised(T value)
    {
      Response?.Invoke(value);
    }
  }
}