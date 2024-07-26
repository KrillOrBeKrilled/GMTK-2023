using System;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
  [CreateAssetMenu(menuName = "Model/Trap/TrapPrefabs", fileName = "TrapPrefabs")]
  public class TrapPrefabs : ScriptableObject {
    public List<TrapPrefabEntry> Traps;
  }

  [Serializable]
  public class TrapPrefabEntry {
    public TrapType Type;
    public GameObject Prefab;
  }
}