using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class FadeInOut : MonoBehaviour {
    [SerializeField] private Graphic _graphic;
    [SerializeField] private float _duration; 

    private void Start() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(this._graphic.DOColor(new Color(1f, 1f, 1f, 0f), this._duration));
      sequence.Append(this._graphic.DOColor(new Color(1f, 1f, 1f, 1f), this._duration));
      sequence.SetLoops(-1);
      sequence.Play();
    }
  }
}