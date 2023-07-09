using TMPro;
using UnityEngine;

public class EndgameUI : MonoBehaviour {
  [SerializeField] private TMP_Text _titleText;
  [SerializeField] private GameObject _nextLevelButton;
  [SerializeField] private GameObject _playAgainButton;

  public void ShowHenWon(string message) {
    this.gameObject.SetActive(true);
    this._titleText.text = message;

    this._nextLevelButton.SetActive(true);
    this._playAgainButton.SetActive(false);
  }

  public void ShowHenLost(string message) {
    this.gameObject.SetActive(true);
    print(message);
    this._titleText.text = message;

    this._nextLevelButton.SetActive(false);
    this._playAgainButton.SetActive(true);
  }
}
