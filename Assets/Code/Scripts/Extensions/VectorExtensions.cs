using UnityEngine;

namespace KrillOrBeKrilled.Extensions {
  public static class VectorExtensions {
    public static Vector3Int RoundToInt(this Vector3 vector) {
      return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }
  }
}