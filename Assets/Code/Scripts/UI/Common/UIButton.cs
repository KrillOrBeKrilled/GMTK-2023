using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using KrillOrBeKrilled.Core.Managers.Audio;

//*******************************************************************************************
// UIButton
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Customized UI Button, plays animation and SFX on tap.
    /// </summary>
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler {
        [Tooltip("Dictates whether or not the button will play SFX when clicked.")]
        [SerializeField] private bool _muteClickSfx;
        [Tooltip("Is button interactable?")]
        [SerializeField] private bool _isInteractable = true;
        [Tooltip("UIButtonTargets that will play a bounce animation and change their sprites on click.")]
        [SerializeField] private List<UIButtonTarget> _targetImages = new();
        [Tooltip("Triggers when the button is pressed down and lifted.")]
        [SerializeField] private UnityEvent _onClick;

        public int TargetImageCount => this._targetImages.Count;
        private bool _isPressed;
        private bool _isPointerOverButton;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        /// <summary>
        /// Triggered by Unity. Invokes <see cref="_onClick"/> event.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData) {
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = false;
            this._targetImages.ForEach(target => {
                target.SetIsPressed(this._isPressed);
                target.ApplySpriteDefault();
            });

            if (this._isPointerOverButton) {
                this._onClick?.Invoke();
            }

            this._isPointerOverButton = false;
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            this._targetImages.ForEach(target => target.DoBounce());
            
            if (!this._muteClickSfx) {
                AudioManager.Instance.PlayUIClick(this.gameObject);
            }
            
            if (!this._isInteractable) {
                return;
            }

            this._isPressed = true;
            this._isPointerOverButton = true;
            this._targetImages.ForEach(target => {
                target.SetIsPressed(this._isPressed);
                target.ApplySpritePressed();
            });
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = true;
            this._targetImages.ForEach(target => target.ApplySpritePressed());
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!this._isPressed) {
                return;
            }

            this._isPointerOverButton = false;
            this._targetImages.ForEach(target => target.ApplySpriteDefault());
        }

        
        /// <summary>
        /// Unity callback triggered when changes are made to the component in the editor.
        /// </summary>
        private void OnValidate() {
            this.SetInteractable(this._isInteractable);
        }

        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods

        /// <summary>
        /// Adds a given target to the targets list.
        /// </summary>
        /// <param name="target">The target to add. </param>
        public void AddTarget(UIButtonTarget target) {
            this._targetImages.Add(target);
        }

        /// <summary>
        /// Sets this button to be interactable or un-interactable.
        /// If un-interactable - will try to set a disabled sprite.
        /// </summary>
        /// <param name="isInteractable"> Value to set the buttons interactable property to. </param>
        public void SetInteractable(bool isInteractable) {
            this._isInteractable = isInteractable;
            this._targetImages.ForEach(target => target.SetInteractable(isInteractable));
        }
        
        #endregion
    }
}
