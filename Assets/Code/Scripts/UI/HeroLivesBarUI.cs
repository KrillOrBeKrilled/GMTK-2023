using Heroes;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************
// HeroLivesBarUI
//*******************************************************************************************
namespace UI {
    /// <summary>
    /// Handles the on-screen positioning and visualization of the hero lives bar UI
    /// related to updates to the associated hero's life count.
    /// </summary>
    public class HeroLivesBarUI : MonoBehaviour {
        [SerializeField] private Hero _hero;
        [SerializeField] private float _heartImageWidth = 8;

        private Image _heartsImage;

        void Awake() {
            this.TryGetComponent(out this._heartsImage);
        }

        void Start() {
            this.UpdateLivesBar(this._hero.Lives);
        }

        void OnEnable() {
            this.UpdateLivesBar(this._hero.Lives);
            this._hero.OnHeroDied.AddListener(this.UpdateLivesBar);
        }

        private void OnDisable() {
            this._hero.OnHeroDied.RemoveListener(this.UpdateLivesBar);
        }

        /// <summary>
        /// Adjusts the hero lives displayed on the screen according to the current hero lives.
        /// </summary>
        /// <param name="lives"> The current hero lives remaining. </param>
        /// <remarks> Subscribed to the <see cref="Hero.OnHeroDied"/> event. </remarks>
        void UpdateLivesBar(int lives, float xPos = 0, float yPos = 0, float zPos = 0) {
            if (this._hero is not null) {
                this._heartsImage.rectTransform.sizeDelta = 
                    new Vector2(this._heartImageWidth * lives, this._heartImageWidth);
            }
        }
    }
}
