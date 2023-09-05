using System.Collections.Generic;
using UnityEngine;

namespace Model {
  [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
  public class LevelData : ScriptableObject {
    public Vector3 EndgameTargetPosition;
    public List<Vector3> RespawnPositions;
    public WavesData WavesData;
  }
}
