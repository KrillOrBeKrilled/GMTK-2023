using System;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.Model {
  [CreateAssetMenu(fileName = "ResourceDropPrefabs", menuName = "Resources/ResourceDropPrefabs")]
  public class ResourceDropPrefabs : ScriptableObject {
    [SerializeField] private List<ResourceDropPrefab> _resourceDrops;

    public GameObject GetResourceDropPrefab(ResourceType type) {
      return this._resourceDrops.Find(resourcePrefab => resourcePrefab.Type == type).Prefab;
    }
  }

  [Serializable]
  public class ResourceDropPrefab {
    public ResourceType Type;
    public GameObject Prefab;
  }
}