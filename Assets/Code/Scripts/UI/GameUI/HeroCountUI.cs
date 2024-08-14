using DG.Tweening;
using KrillOrBeKrilled.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class HeroCountUI : MonoBehaviour {
    [SerializeField] private TMP_Text _heroCount;
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _endlessIcon;
    [SerializeField] private Sprite _storyIcon;

    private Tween _countTween;

    public void Initialize(bool isEndless) {
      this._icon.sprite = isEndless ? this._endlessIcon : this._storyIcon;
    }

    public void UpdateHeroCount(int newAmount) {
      if (newAmount < 0) {
        return;
      } 
      
      int oldAmount = int.Parse(this._heroCount.text);
      if (newAmount < oldAmount) {
        this._heroCount.text = $"{newAmount}";
      } else {
        this._countTween?.Kill();
        this._countTween = this._heroCount.DoCount(newAmount, 0.5f);
      }
    }

    private void Awake() {
      this._heroCount.text = "0";
    }
  }
}