using System.Collections.Generic;
using System.Linq;
using KrillOrBeKrilled.Model;
using UnityEngine;

//*******************************************************************************************
// ResourceIconData
//*******************************************************************************************
namespace KrillOrBeKrilled.Traps {
    /// <summary>
    /// Stores data of all <see cref="ResourceTypeIcon"/> used to represent every
    /// resource Type in the system.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceIconData", menuName = "UI Data/ResourceIconData")]
    public class ResourceIconData : ScriptableObject {
        [SerializeField] private List<ResourceTypeIcon> _icons;

        /// <summary>
        /// Finds the resource icon associated with the provided <see cref="ResourceType"/> data.
        /// </summary>
        /// <param name="type"> The <see cref="ResourceType"/> used to find the related icon. </param>
        /// <returns> The resource icon sprite associated with the provided resource Type data. </returns>
        public Sprite TypeToImage(ResourceType type) {
            return this._icons.Find(typeIcon => typeIcon.Type == type).Icon;
        }
    }
}
