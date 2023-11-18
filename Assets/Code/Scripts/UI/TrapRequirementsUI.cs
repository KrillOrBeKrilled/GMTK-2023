using DG.Tweening;
using KrillOrBeKrilled.Traps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class TrapRequirementsUI : MonoBehaviour {
    [SerializeField] private Image _trapIcon;
    [SerializeField] private List<ResourceRequirementUI> _trapRequirements;
    [SerializeField] private TrapIconData _trapIconData;

    private Sequence _tweenSequence;
    private const float ScaleUpValue = 1.05f;
    private const float AnimationDuration = 0.05f;
    private RectTransform _rectTransform;

    //========================================
    // Unity Methods
    //========================================

    private void Awake() {
      this._rectTransform = (RectTransform)this.transform;
    }

    private void Start() {
      this.gameObject.SetActive(false);
      this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 5);
      this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 6);
      this._trapRequirements[2].SetIconAmount(ResourceType.Slime, 7);
    }


    //========================================
    // Public Methods
    //========================================
    public void Initialize(UnityEvent<Trap> trapChanged) {
      trapChanged.AddListener(this.SetTrap);
    }

    public void ToggleActive() {
      if (this.gameObject.activeSelf) {
        this.gameObject.SetActive(false);
        return;
      }

      this.gameObject.SetActive(true);
      this.DoBounceAnimation();
    }

    private void DoBounceAnimation() {
      this._tweenSequence?.Kill();
      this._tweenSequence = DOTween.Sequence();
      this._tweenSequence.Append(this._rectTransform.DOScale(new Vector3(ScaleUpValue, ScaleUpValue, 1f), AnimationDuration));
      this._tweenSequence.Append(this._rectTransform.DOScale(Vector3.one, AnimationDuration));
      this._tweenSequence.OnComplete(() => {
        this._tweenSequence = null;
      });
      this._tweenSequence.SetEase(Ease.InOutSine);
      this._tweenSequence.Play();
    }

    private void SetTrap(Trap trap) {
      this._trapIcon.sprite = this._trapIconData.TrapToImage(trap);

      switch (trap) {
        // TODO: replace this logic later to use new resource system
        case SpikeTrap:
          this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 2);
          this._trapRequirements[1].Hide();
          this._trapRequirements[2].Hide();
          break;
        case SwingingAxeTrap:
          this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 1);
          this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 1);
          this._trapRequirements[1].SetIconAmount(ResourceType.Dynamite, 1);
          break;
        case IcicleTrap:
          this._trapRequirements[0].SetIconAmount(ResourceType.IceShards, 2);
          this._trapRequirements[1].SetIconAmount(ResourceType.Slime, 1);
          this._trapRequirements[2].Hide();
          break;
        case AcidPitTrap:
          this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 1);
          this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 1);
          this._trapRequirements[2].SetIconAmount(ResourceType.Slime, 4);
          break;
        default:
          this._trapRequirements[0].Hide();
          this._trapRequirements[1].Hide();
          this._trapRequirements[2].Hide();
          break;
      }
    }
  }
}
