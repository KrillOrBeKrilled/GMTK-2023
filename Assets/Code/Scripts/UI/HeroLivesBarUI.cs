using Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HeroLivesBarUI : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private float _heartImageWidth = 8;

        private Image _heartsImage;

        void Awake()
        {
            this.TryGetComponent(out this._heartsImage);
        }

        void Start()
        {
            this.UpdateLivesBar(this._hero.Lives);
        }

        void OnEnable()
        {
            this.UpdateLivesBar(this._hero.Lives);
            this._hero.OnHeroDied.AddListener(this.UpdateLivesBar);
        }

        private void OnDisable()
        {
            this._hero.OnHeroDied.RemoveListener(this.UpdateLivesBar);
        }

        void UpdateLivesBar(int lives, float xPos = 0, float yPos = 0, float zPos = 0)
        {
            if (this._hero != null)
            {
                this._heartsImage.rectTransform.sizeDelta = new Vector2(this._heartImageWidth * lives, this._heartImageWidth);
            }
        }
    }
}
