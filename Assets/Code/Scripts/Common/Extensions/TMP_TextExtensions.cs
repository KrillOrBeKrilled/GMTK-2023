using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace KrillOrBeKrilled.Common {
  public static class TMPTextExtensions {
    public static Tween DoCount(this TMP_Text text, int to, float duration) {
        int from = 0;
        try {
          from = int.Parse(text.text);
        } catch (ArgumentException) {
          Debug.LogError("DoCount tween: error while attempting to parse the TMP_Text's text field");
          return null;
        }
        
        return DOVirtual
          .Int(from, to, duration, newValue => {
            text.text = $"{newValue}";
          })
          .SetEase(Ease.InOutSine);
      }
    
      public static Tween DoCount(this TMP_Text text, int from, int to, float duration) {
        return DOVirtual
          .Int(from, to, duration, newValue => text.text = $"{newValue}")
          .SetEase(Ease.InOutSine);
      }
  }
}