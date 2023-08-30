using Heroes;
using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class MapUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image _playerIcon;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Transform _heroIconsParent;
    [SerializeField] private RectTransform _minPosition;
    [SerializeField] private RectTransform _maxPosition;

    [Header("Hero Icon Prefabs")]
    [SerializeField] private Image _heroIconPrefab;

    private Dictionary<Hero, Image> _heroToIconDict;
    private PlayerManager _player;
    private float _sliderLength;

    public void Initialize(PlayerManager player) {
      this._player = player;
    }

    public void RegisterHero(Hero newHero) {
      newHero.OnHeroDied.AddListener(this.UnregisterHero);
      Image newIcon = Instantiate(this._heroIconPrefab, this._heroIconsParent);
      this._heroToIconDict.Add(newHero, newIcon);
    }

    private void Awake() {
      this._progressSlider.value = 0f;
      this._heroToIconDict = new Dictionary<Hero, Image>();
      this._sliderLength = this._maxPosition.anchoredPosition.x - this._minPosition.anchoredPosition.x;
    }

    private void Update() {
      // Update icon position of each hero based on their progress
      float greatestProgress = 0f;
      foreach (KeyValuePair<Hero,Image> kvp in this._heroToIconDict) {
        Hero hero = kvp.Key;
        Image image = kvp.Value;

        image.rectTransform.anchoredPosition = new Vector2(this._sliderLength * hero.MapPosition, 0);
        greatestProgress = Mathf.Max(greatestProgress, hero.MapPosition);
      }

      this._progressSlider.value = greatestProgress;

      if (this._player is not null) {
        this._playerIcon.rectTransform.anchoredPosition = new Vector2(this._sliderLength * this._player.MapPosition, 0);
      }
    }

    private void UnregisterHero(Hero diedHero) {
      if (this._heroToIconDict.TryGetValue(diedHero, out Image diedHeroIcon)) {
        Destroy(diedHeroIcon.gameObject);
      }

      this._heroToIconDict.Remove(diedHero);
    }
  }
}
