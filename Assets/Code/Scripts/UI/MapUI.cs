using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
    public class MapUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image _playerIcon;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Transform _heroIconsParent;
        [SerializeField] private RectTransform _minPosition;
        [SerializeField] private RectTransform _maxPosition;

        [Header("Color Settings")]
        [SerializeField] private Image _fillArea;
        [SerializeField] private Color _safeColor;
        [SerializeField] private Color _dangerColor;

        [Header("Hero Icon Prefabs")]
        [SerializeField] private Image _defaultHeroIconPrefab;
        [SerializeField] private Image _druidHeroIconPrefab;

        private readonly Dictionary<Hero, Image> _heroToIconDict = new Dictionary<Hero, Image>();
        private Transform _player;
        private float _sliderLength;

        private float _levelStartX;
        private float _levelEndX;

        public void Initialize(Transform player, float levelStartX, float levelEndX) {
            this._player = player;
            this._levelStartX = levelStartX;
            this._levelEndX = levelEndX;
        }

        public void RegisterHero(Hero newHero) {
            newHero.OnHeroDied.AddListener(this.UnregisterHero);
            Image heroIconPrefab;

            switch (newHero.Type) {
                case HeroData.HeroType.Default:
                case HeroData.HeroType.AcidResistant:
                case HeroData.HeroType.Armoured:
                    heroIconPrefab = this._defaultHeroIconPrefab;
                    break;
                case HeroData.HeroType.Druid:
                    heroIconPrefab = this._druidHeroIconPrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Image newIcon = Instantiate(heroIconPrefab, this._heroIconsParent);
            this._heroToIconDict.Add(newHero, newIcon);
        }

        private void Awake() {
            this._progressSlider.value = 0f;
            this._sliderLength = this._maxPosition.anchoredPosition.x - this._minPosition.anchoredPosition.x;
        }

        private void Update() {
            // Update icon position of each hero based on their progress
            float greatestProgress = 0f;
            foreach (KeyValuePair<Hero,Image> kvp in this._heroToIconDict) {
                Hero hero = kvp.Key;
                Image image = kvp.Value;

                float heroMapProgress = this.GetHeroMapProgress(hero);
                image.rectTransform.anchoredPosition = new Vector2(this._sliderLength * heroMapProgress, 0);
                greatestProgress = Mathf.Max(greatestProgress, heroMapProgress);
            }

            this.SetFillAreaColor(greatestProgress);

            if (this._player != null) {
                this._playerIcon.rectTransform.anchoredPosition = 
                    new Vector2(this._sliderLength * this.GetHeroMapProgress(this._player), 0);
            }
        }

        private void SetFillAreaColor(float greatestHeroProgress) {
            this._progressSlider.value = greatestHeroProgress;
            this._fillArea.color = Color.Lerp(this._safeColor, this._dangerColor, greatestHeroProgress);
        }

        private void UnregisterHero(Hero diedHero) {
            if (this._heroToIconDict.TryGetValue(diedHero, out Image diedHeroIcon)) {
                Destroy(diedHeroIcon.gameObject);
            }

            this._heroToIconDict.Remove(diedHero);
        }

        private float GetHeroMapProgress(Component character) {
            float mapProgress = (character.transform.position.x - this._levelStartX) / (this._levelEndX - this._levelStartX);
            return Mathf.Clamp(mapProgress, 0f, 1f);
        }
    }
}
