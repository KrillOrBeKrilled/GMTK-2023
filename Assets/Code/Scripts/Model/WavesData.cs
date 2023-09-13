using System;
using System.Collections.Generic;

//*******************************************************************************************
// WavesData
//*******************************************************************************************
namespace KrillOrBeKrilled.Model {
    /// <summary>
    /// Stores data on all <see cref="WaveData"/> in a set of waves.
    /// </summary>
    [Serializable]
    public class WavesData {
        public List<WaveData> WavesList;
    }
}
