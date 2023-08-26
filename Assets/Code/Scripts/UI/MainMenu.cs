using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class MainMenu : MonoBehaviour {
    [SerializeField] private Image _foreground;

    private const float FadeDuration = 0.5f;

    public void OnStartGame() {
      this._foreground.gameObject.SetActive(true);
      this._foreground
        .DOFade(1, FadeDuration)
        .OnComplete(SceneNavigationManager.Instance.LoadGameScene);
    }

    private void Awake() {
      this._foreground.gameObject.SetActive(true);
      this._foreground
        .DOFade(0, FadeDuration)
        .OnComplete(() => this._foreground.gameObject.SetActive(false));
    }
  }
}
