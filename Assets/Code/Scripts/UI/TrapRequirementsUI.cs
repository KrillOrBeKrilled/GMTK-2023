using DG.Tweening;
using KrillOrBeKrilled.Traps;
using System.Collections.Generic;
using UnityEngine;

namespace KrillOrBeKrilled.UI {
  public class TrapRequirementsUI : MonoBehaviour {
    [SerializeField] private List<TrapRequirementUI> _trapRequirements;

    private Sequence _tweenSequence;
    private const float ScaleUpValue = 1.05f;
    private const float ScaleDownValue = 0.95f;
    private const float AnimationDuration = 0.05f;
    private RectTransform _rectTransform;

    //========================================
    // Unity Methods
    //========================================

    private void Awake() {
      this._rectTransform = (RectTransform)this.transform;
    }

    private void Start() {
      this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 5);
      this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 6);
      this._trapRequirements[2].SetIconAmount(ResourceType.Slime, 7);
    }


    //========================================
    // Public Methods
    //========================================
    public void ToggleActive() {
      if (this.gameObject.activeSelf) {
        this.gameObject.SetActive(false);
        return;
      }

      this.gameObject.SetActive(true);
      this._tweenSequence?.Kill();
      this._tweenSequence = DOTween.Sequence();
      this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
      //this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleDownValue, ScaleDownValue, 1f), AnimationDuration));
      this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
      this._tweenSequence.OnComplete(() => {
        this._tweenSequence = null;

      });
      this._tweenSequence.SetEase(Ease.InOutSine);
      this._tweenSequence.Play();
    }
  }
}
