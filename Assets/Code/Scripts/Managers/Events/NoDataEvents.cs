using UnityEngine.Events;

namespace KrillOrBeKrilled.Managers {
  //*******************************************************************************************
  // GameOverEvent
  //*******************************************************************************************
  /// <summary>
  /// A callback that tracks when the game has concluded.
  /// </summary>
  public class GameOverEvent : UnityEvent {}

  public class ShowDialogueUIEvent : UnityEvent {}

  public class HideDialogueUIEvent : UnityEvent {}
}
