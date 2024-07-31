using DG.Tweening;
using TMPro;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class WavesUI : MonoBehaviour {
    [SerializeField] private TMP_Text _leftWavesCount;

    private Sequence _fadeSequence;

    public void UpdateWavesCount(int amount) {
      this._leftWavesCount.text = amount == 1 ? "Last Wave!" : $"{amount} Waves Left";
      this._leftWavesCount.gameObject.SetActive(true);
      
      this._fadeSequence?.Kill();
      
      this._fadeSequence = DOTween.Sequence();
      this._fadeSequence.Append(this._leftWavesCount.DOFade(1f, 0.5f));
      this._fadeSequence.AppendInterval(1f);
      this._fadeSequence.Append(this._leftWavesCount.DOFade(0f, 1f));
      this._fadeSequence.OnComplete(() => this._leftWavesCount.gameObject.SetActive(false));
      this._fadeSequence.Play();
    } 
  }
}