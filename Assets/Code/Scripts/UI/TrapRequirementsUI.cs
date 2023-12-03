using DG.Tweening;
using KrillOrBeKrilled.Traps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//*******************************************************************************************
// TrapRequirementsUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages the trap resource requirement window display, updating the trap type
    /// requirements upon various trap selections from each individual required
    /// <see cref="ResourceAmountUI">resource</see> to the associated trap icon.
    /// </summary>
    /// <remarks>
    /// Resizes the window display according to the number of unique resource
    /// types displayed.
    /// </remarks>
    public class TrapRequirementsUI : MonoBehaviour {
        [Tooltip("Displays the icon of the selected trap type.")] 
        [SerializeField] private Image _trapIcon;
        [Tooltip("Displays each unique resource requirement of the selected trap type.")] 
        [SerializeField] private List<ResourceAmountUI> _trapRequirements;
        [Tooltip("The cumulative trap icon data used to find the icon to display for the selected trap type.")] 
        [SerializeField] private TrapIconData _trapIconData;

        private Sequence _tweenSequence;
        private const float ScaleUpValue = 1.05f;
        private const float AnimationDuration = 0.05f;
        private RectTransform _rectTransform;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods

        private void Awake() {
            this._rectTransform = (RectTransform)this.transform;
        }

        private void Start() {
            this.gameObject.SetActive(false);
            this._trapRequirements[0].SetIconAmount(ResourceType.ScrapMetal, 5);
            this._trapRequirements[1].SetIconAmount(ResourceType.WoodStick, 6);
            this._trapRequirements[2].SetIconAmount(ResourceType.Slime, 7);
        }

        #endregion

        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Sets up all listeners to operate the trap requirement widget UI.
        /// </summary>
        /// <param name="trapChanged"> The event associated with updates to the trap selection. </param>
        public void Initialize(UnityEvent<Trap> trapChanged) {
            trapChanged.AddListener(this.SetTrap);
        }

        /// <summary>
        /// Enables the trap requirement UI GameObject and plays a scaling bounce animation.
        /// </summary>
        public void ToggleActive() {
            if (this.gameObject.activeSelf) {
              this.gameObject.SetActive(false);
              return;
            }

            this.gameObject.SetActive(true);
            this.DoBounceAnimation();
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Scales the trap requirement UI up and back down to simulate a bounce animation.
        /// </summary>
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

        /// <summary>
        /// Updates the trap icon and each required <see cref="ResourceAmountUI"/> display, adjusting the UI window
        /// size accordingly.
        /// </summary>
        /// <param name="trap"> The trap class used to determine the trap type to update the UI accordingly. </param>
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
                    this._trapRequirements[2].Hide();
                break;
                    case IcicleTrap:
                    this._trapRequirements[0].SetIconAmount(ResourceType.IceShards, 2);
                    this._trapRequirements[1].SetIconAmount(ResourceType.Dynamite, 1);
                    this._trapRequirements[2].Hide();
                    break;
                case AcidPitTrap:
                    this._trapRequirements[0].SetIconAmount(ResourceType.Slime, 1);
                    this._trapRequirements[1].SetIconAmount(ResourceType.IceShards, 2);
                    this._trapRequirements[2].SetIconAmount(ResourceType.Dynamite, 1);
                    break;
                default:
                    this._trapRequirements[0].Hide();
                    this._trapRequirements[1].Hide();
                    this._trapRequirements[2].Hide();
                    break;
            }
        }
        
        #endregion
    }
}
