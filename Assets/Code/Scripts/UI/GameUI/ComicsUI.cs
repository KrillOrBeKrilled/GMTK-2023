using System.Collections.Generic;
using KrillOrBeKrilled.Common;
using UnityEngine;
using UnityEngine.UI;

namespace KrillOrBeKrilled.UI {
  public class ComicsUI : MonoBehaviour {
    [SerializeField] private Image _comicImage;
    [SerializeField] private GameObject _comicContainer;
    [SerializeField] private GameEvent _onComicsComplete;
    
    private List<Sprite> _comicSprites;
    private int _comicIndex;

    public void LoadComics(List<Sprite> comicSprites) {
      this._comicSprites = comicSprites;
      this.ShowPage(0);
    }
    
    public void ShowNextPage() {
      print("Next");
      this.ShowPage(this._comicIndex + 1);
    }
    
    public void ShowComics() {
      this._comicContainer.SetActive(true);
    }
    
    private void HideComics() {
      this._comicContainer.SetActive(false);
    }

    private void ShowPage(int index) {
      if (index >= this._comicSprites.Count) {
        this._onComicsComplete.Raise();
        this.HideComics();
        return;
      }

      this._comicIndex = index;
      this._comicImage.sprite = this._comicSprites[this._comicIndex];
    }
  }
}