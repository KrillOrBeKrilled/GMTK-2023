using DG.Tweening;
using Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class HealthBarUI : MonoBehaviour {
    [SerializeField] Vector2 _positionOffset;

    private Camera _mainCamera;
    private RectTransform _rectTransform;
    private Slider _healthBar;

    private Transform _objectToFollow;
    private Hero _hero;
    private RectTransform _targetCanvas;

    private Tween _sliderTween;
    private Tween _moveTween;

    public void Initialize(Hero targetHero, RectTransform targetCanvas) {
      this._hero = targetHero;
      this._objectToFollow = targetHero.transform;
      this._targetCanvas = targetCanvas;

      this._hero.OnHealthChanged.AddListener(this.OnHealthChanged);
      this._hero.OnHeroDied.AddListener(this.OnDeath);

      this._healthBar.maxValue = targetHero.Health;
      this._healthBar.value = targetHero.Health;
    }

    private void Awake() {
      this._mainCamera = Camera.main;
      this._healthBar = this.GetComponent<Slider>();
      this._rectTransform = this.GetComponent<RectTransform>();

      this._healthBar.maxValue = 0;
      this._healthBar.value = 0;
      this._healthBar.minValue = 0;

      this.RepositionHealthBar();
    }

    private void Update() {
      this.RepositionHealthBar();
    }

    private void OnHealthChanged(int health)
    {
      var tweenDuration = 0f;

      this._sliderTween?.Kill();
      this._sliderTween = this._healthBar
        .DOValue(health, tweenDuration)
        .SetEase(Ease.InOutCubic);
    }

    private void OnDisable()
    {
      this._hero.OnHealthChanged.RemoveListener(this.OnHealthChanged);
      this._hero.OnHeroDied.RemoveListener(this.OnDeath);
    }

    private void OnDeath(Hero _) {
      Destroy(this.gameObject);
    }

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
