using DG.Tweening;
using TMPro;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class FadeInOutText : MonoBehaviour {
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _duration; 

    private void Start() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(this._text.DOColor(new Color(1f, 1f, 1f, 0f), this._duration));
      sequence.Append(this._text.DOColor(new Color(1f, 1f, 1f, 1f), this._duration));
      sequence.SetLoops(-1);
      sequence.Play();
    }
  }
}