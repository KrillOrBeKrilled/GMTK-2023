using DG.Tweening;
using KrillOrBeKrilled.Heroes;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// HeroHealthBarUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the on-screen positioning of the hero health bar UI and visualizes
    /// updates to the associated hero's health.
    /// </summary>
    public class HealthBarUI : MonoBehaviour {
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

            this._healthBar.maxValue = 0;
            this._healthBar.value = 0;
            this._healthBar.minValue = 0;

            this.RepositionHealthBar();
        }

        public void Initialize(Hero targetHero, RectTransform targetCanvas) {
            this._hero = targetHero;
            this._objectToFollow = targetHero.transform;
            this._targetCanvas = targetCanvas;

            this._hero.OnHealthChanged.AddListener(this.OnHealthChanged);
            this._hero.OnHeroDied.AddListener(this.OnDeath);

            this._healthBar.maxValue = targetHero.Health;
            this._healthBar.value = targetHero.Health;
        }
        
        private void Update() {
            this.RepositionHealthBar();
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
        private void OnDeath(Hero _) {
            Destroy(this.gameObject);
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

            // this._rectTransform.DOAnchorPos(worldObjectScreenPosition, 0.1f);
            this._rectTransform.anchoredPosition = worldObjectScreenPosition;
        }
    }
}
