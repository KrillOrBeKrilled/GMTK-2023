using DG.Tweening;
using KrillOrBeKrilled.Heroes;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// HeroHealthBarUI
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Handles the on-screen positioning of the hero health bar UI and visualizes
    /// updates to the associated hero's health.
    /// </summary>
    public class HeroHealthBarUI : MonoBehaviour {
        [Tooltip("Offsets the health bar from the _objectToFollow position.")]
        [SerializeField] Vector2 _positionOffset;
        [Tooltip("Provides events related to the hero status to subscribe to.")]
        [SerializeField] private Hero _hero;
        [Tooltip("Provides position data to constantly update the health bar to follow this target.")]
        [SerializeField] private Transform _objectToFollow;
        [Tooltip("Translates the health bar onto the canvas during movement.")]
        [SerializeField] private RectTransform _targetCanvas;

        private Camera _mainCamera;
        private RectTransform _rectTransform;
        private Slider _healthBar;

        private Tween _sliderTween;
        private Tween _moveTween;

        private void Awake() {
            this._mainCamera = Camera.main;
            this._healthBar = this.GetComponent<Slider>();
            this._rectTransform = this.GetComponent<RectTransform>();

            this._healthBar.maxValue = Hero.MaxHealth;
            this._healthBar.value = Hero.MaxHealth;
            this._healthBar.minValue = 0;

            this.RepositionHealthBar();
        }
        
        private void Update() {
            this.RepositionHealthBar();
        }

        private void OnEnable() {
            this._hero.OnHealthChanged.AddListener(this.OnHealthChanged);
            this._hero.OnHeroDied.AddListener(this.OnDeath);
        }
        
        private void OnDisable() {
            this._hero.OnHealthChanged.RemoveListener(this.OnHealthChanged);
            this._hero.OnHeroDied.RemoveListener(this.OnDeath);
        }

        /// <summary>
        /// Sets the hero health bar slider value with a new tween.
        /// </summary>
        /// <param name="health"> The new health bar value. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHealthChanged"/> event. </remarks>
        private void OnHealthChanged(int health) {
            var tweenDuration = 0f;

            this._sliderTween?.Kill();
            this._sliderTween = this._healthBar
                .DOValue(health, tweenDuration)
                .SetEase(Ease.InOutCubic);
        }
        
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
        private void OnDeath(int numberLives, float xPos = 0, float yPos = 0, float zPos = 0) {
            
        }

        /// <summary>
        /// Updates the hero health bar UI position according to the position of <see cref="_objectToFollow"/>.
        /// </summary>
        /// <remarks> Does nothing if <see cref="_objectToFollow"/> is null. </remarks>
        private void RepositionHealthBar() {
            if (this._objectToFollow == null)
                return;

            Vector2 viewportPosition = this._mainCamera.WorldToViewportPoint(this._objectToFollow.position);
            Vector2 sizeDelta = this._targetCanvas.sizeDelta;
            Vector2 worldObjectScreenPosition =
            new Vector2(viewportPosition.x * sizeDelta.x - sizeDelta.x * 0.5f,
              viewportPosition.y * sizeDelta.y - sizeDelta.y * 0.5f);
            worldObjectScreenPosition += this._positionOffset;

            this._moveTween?.Kill();
            this._moveTween = this._rectTransform.DOAnchorPos(worldObjectScreenPosition, 0.1f);
        }
    }
}
