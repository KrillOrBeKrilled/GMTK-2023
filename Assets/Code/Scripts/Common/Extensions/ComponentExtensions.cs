using UnityEngine;

namespace KrillOrBeKrilled.Common {
  public static class ComponentExtensions {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponentExactlyInChildren<T>(this Transform parent) {
      for (int i = 0; i < parent.childCount; i++) {
        Transform child = parent.GetChild(i);
        
        if (child.TryGetComponent(out T target)) {
          return target;
        }
      }

      return default;
    }
  }
}