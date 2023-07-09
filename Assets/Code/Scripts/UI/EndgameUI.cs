using UnityEngine;

public class EndgameUI : MonoBehaviour {
  [SerializeField] private GameObject _winText;
  [SerializeField] private GameObject _lossText;

  public void Show(bool wonGame) {
    this.gameObject.SetActive(true);
    this._winText.SetActive(wonGame);
    this._lossText.SetActive(!wonGame);
  }
}
