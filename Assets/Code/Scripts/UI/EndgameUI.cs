using UnityEngine;

public class EndgameUI : MonoBehaviour {
  [SerializeField] private GameObject _henWinText;
  [SerializeField] private GameObject _henDiedText;
  [SerializeField] private GameObject _heroReachLevelEndText;
  [SerializeField] private GameObject _nextLevelButton;
  [SerializeField] private GameObject _playAgainButton;

  public void ShowHenWon() {
    this.gameObject.SetActive(true);
    this._henWinText.SetActive(true);

    this._nextLevelButton.SetActive(true);
    this._playAgainButton.SetActive(false);
  }

  public void ShowHenDied() {
    this.gameObject.SetActive(true);
    this._henDiedText.SetActive(true);

    this._nextLevelButton.SetActive(false);
    this._playAgainButton.SetActive(true);
  }

  public void ShowHeroReachedLevelEnd() {
    this.gameObject.SetActive(true);
    this._heroReachLevelEndText.SetActive(true);

    this._nextLevelButton.SetActive(false);
    this._playAgainButton.SetActive(true);
  }
}
