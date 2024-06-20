using KrillOrBeKrilled.Heroes;
using KrillOrBeKrilled.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// MapUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manages the positioning of the hero and player icons on the map UI in real
    /// time. Exposes methods to register new spawned heroes and supports the
    /// representation of multiple types of heroes with unique icons for each type.
    /// </summary>
    public class MapUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Image _playerIcon;
        [Tooltip("Tracks the distance covered by the heroes with respect to the level start and goal positions.")] 
        [SerializeField] private Slider _progressSlider;
        [Tooltip("Holds all references to the active hero icons.")]
        [SerializeField] private Transform _heroIconsParent;

        [Header("Color Settings")]
        [SerializeField] private Image _fillArea;
        [Tooltip("The color of the hero progress slider fill when the hero is far from the goal.")] 
        [SerializeField] private Color _safeColor;
        [Tooltip("The color of the hero progress slider fill when the hero is close to the goal.")] 
        [SerializeField] private Color _dangerColor;

        [Header("Hero Icon Prefabs")]
        [SerializeField] private Image _defaultHeroIconPrefab;
        [SerializeField] private Image _druidHeroIconPrefab;

        /// <summary> Links each <see cref="Hero"/> to its icon representation on the map UI. </summary>
        private readonly Dictionary<Hero, Image> _heroToIconDict = new Dictionary<Hero, Image>();
        private Transform _player;
        private float _sliderLength;

        private float _levelStartX;
        private float _levelEndX;

        /// <summary>
        /// The padding of the hero and hen icons on the map.
        /// Padding value will be applied from the left and right.
        /// </summary>
        private const int IconPositionPadding = 15;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Start() {
            this._progressSlider.value = 0f;
            this._sliderLength = ((RectTransform)this._progressSlider.transform).rect.width - IconPositionPadding * 2;
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

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Sets up all references to operate the map UI.
        /// </summary>
        /// <param name="player"> The player GameObject <see cref="Transform"/> to track for positional changes. </param>
        /// <param name="levelStartX"> The beginning position of the level along the x-axis. </param>
        /// <param name="levelEndX"> The end position of the level along the x-axis. </param>
        public void Initialize(Transform player, float levelStartX, float levelEndX) {
            this._player = player;
            this._levelStartX = levelStartX;
            this._levelEndX = levelEndX;
        }

        /// <summary>
        /// Adds the <see cref="Hero"/> to bookkeeping data structures and listeners and assigns it an icon to be
        /// represented on the map UI.
        /// </summary>
        /// <param name="newHero"> The <see cref="Hero"/> to be added to the map UI. </param>
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

            Image newIcon = Instantiate(heroIconPrefab, this._heroIconsParent, true);
            newIcon.rectTransform.localScale = Vector3.one;
            newIcon.transform.SetAsFirstSibling();
            this._heroToIconDict.Add(newHero, newIcon);
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        /// <summary>
        /// Helper method for <see cref="Update"/> that calculates the progress of a GameObject between the start
        /// and end goal of the level as a percentage.
        /// </summary>
        /// <param name="character"> A component associated with the GameObject to get the position data from. </param>
        /// <returns> The distance covered towards the level end goal as a percentage. </returns>
        private float GetHeroMapProgress(Component character) {
            float mapProgress = (character.transform.position.x - this._levelStartX) / (this._levelEndX - this._levelStartX);
            return Mathf.Clamp(mapProgress, 0f, 1f);
        }

        /// <summary>
        /// Updates the fill amount and color of the map UI slider according to the greatest distance percentage
        /// covered by the heroes.
        /// </summary>
        /// <param name="greatestHeroProgress"> The greatest distance covered by the heroes as a percentage. </param>
        /// <remarks> Renders a color spectrum from <see cref="_safeColor"/> to <see cref="_dangerColor"/> based
        /// on the <see cref="greatestHeroProgress"/>. </remarks>
        private void SetFillAreaColor(float greatestHeroProgress) {
            this._progressSlider.value = greatestHeroProgress;
            this._fillArea.color = Color.Lerp(this._safeColor, this._dangerColor, greatestHeroProgress);
        }

        /// <summary>
        /// Removes the <see cref="Hero"/> from bookkeeping data structures and its associated icon from the map UI
        /// if this <see cref="Hero"/> is currently registered in the bookkeeping data structures.
        /// </summary>
        /// <param name="diedHero"> The <see cref="Hero"/> to be removed from the map UI. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
        private void UnregisterHero(Hero diedHero) {
            if (this._heroToIconDict.TryGetValue(diedHero, out Image diedHeroIcon)) {
                Destroy(diedHeroIcon.gameObject);
            }

            this._heroToIconDict.Remove(diedHero);
        }

        #endregion
    }
}
