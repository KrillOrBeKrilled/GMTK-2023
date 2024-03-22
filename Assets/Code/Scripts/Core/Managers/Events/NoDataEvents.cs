using UnityEngine.Events;

namespace KrillOrBeKrilled.Core.Managers {
  //*******************************************************************************************
  // GameOverEvent
  //*******************************************************************************************
  /// <summary>
  /// A callback that tracks when the game has concluded.
  /// </summary>
  public class GameOverEvent : UnityEvent {}

  public class ShowDialogueUIEvent : UnityEvent {}

  public class EndDialogueEvent : UnityEvent {}
}
