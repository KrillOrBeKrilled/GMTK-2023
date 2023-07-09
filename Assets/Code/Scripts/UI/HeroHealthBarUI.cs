using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealthBarUI : MonoBehaviour {
  [SerializeField] Vector2 _positionOffset;
  [SerializeField] private Hero _hero;
  [SerializeField] private Transform _objectToFollow;
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

  private void OnEnable()
  {
    this._hero.OnHealthChanged.AddListener(this.OnHealthChanged);
    this._hero.OnHeroDied.AddListener(this.OnDeath);
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

  private void OnDeath() {
  }

  private void RepositionHealthBar() {
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
