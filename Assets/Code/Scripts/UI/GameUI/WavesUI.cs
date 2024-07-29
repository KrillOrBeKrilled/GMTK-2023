using TMPro;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class WavesUI : MonoBehaviour {
    [SerializeField] private TMP_Text _leftWavesCount;

    public void UpdateWavesCount(int amount) {
      this._leftWavesCount.text = amount == 1 ? "Last Wave!" : $"{amount} Waves Left";
    } 
  }
}