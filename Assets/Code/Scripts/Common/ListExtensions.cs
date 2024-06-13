using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace KrillOrBeKrilled.Extensions {
  public static class ListExtensions {
    public static T GetRandomElement<T>(this List<T> list) {
      if (list.Count <= 0)
        throw new IndexOutOfRangeException();

      int index = Random.Range(0, list.Count);
      return list[index];
    }

    public static void RemoveRange<T>(this List<T> list, IEnumerable<T> range) {
      foreach (T target in range) {
        list.Remove(target);
      }
    }
  }
}