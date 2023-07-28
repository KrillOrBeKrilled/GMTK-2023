using Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class GameManager : Singleton<GameManager> {
  [SerializeField] private GameUI _gameUI;
  [SerializeField] private Player _player;
  [SerializeField] private Hero _hero;
  [SerializeField] private EndgameTarget _endgameTarget;
  [SerializeField] private OutOfBoundsTrigger _outOfBoundsTrigger;
  [SerializeField] private Vector2 _playerRespawnOffset;

  public UnityEvent OnSetupComplete { get; private set; }
  public UnityEvent<string> OnHenWon { get; private set; }
  public UnityEvent<string> OnHenLost { get; private set; }

  public void LoadMainMenu() {
    PauseManager.Instance.UnpauseGame();
    PauseManager.Instance.SetIsPausable(false);
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.LoadMainMenuScene);
  }

  public void LoadNextLevel() {
    PauseManager.Instance.UnpauseGame();
    print("Loading Next Level");
  }

  public void ReloadThisLevel() {
    PauseManager.Instance.UnpauseGame();
    PauseManager.Instance.SetIsPausable(false);
    this._gameUI.FadeInSceneCover(SceneNavigationManager.Instance.ReloadCurrentScene);
  }

  protected override void Awake() {
    base.Awake();
    this.OnSetupComplete = new UnityEvent();
    this.OnHenWon = new UnityEvent<string>();
    this.OnHenLost = new UnityEvent<string>();
  }

  [YarnCommand("enter_player")]
  public void EnterPlayer()
  {
    _hero.EnterLevel();
  }

  [YarnCommand("start_level")]
  public void StartLevel()
  {
    _hero.StartRunning();
    _outOfBoundsTrigger.ToggleBounds(true);
    CoinManager.Instance.StartCoinEarning();
  }

  private void Start() {
    // Setup
    this._gameUI.Initialize(this, this._player);

    _hero.ResetHero();
    _outOfBoundsTrigger.ToggleBounds(false);

    this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
    this._player.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
    this._player.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);
    this._outOfBoundsTrigger.OnPlayerOutOfBounds.AddListener(this.HenOutOfBounds);
    this._hero.OnGameOver.AddListener(this.GameWon);
    this._hero.OnHeroDied.AddListener(this.OnHeroDied);

    this.OnSetupComplete?.Invoke();
    PauseManager.Instance.SetIsPausable(true);
  }

  private void OnPlayerStateChanged(IPlayerState state, float xPos, float yPos, float zPos) {
    if (state is GameOverState) {
      // Send Analytics data before ending the game
      UGS_Analytics.PlayerDeathByHeroCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
      
      this.HenDied("The Hero managed to take you down Hendall.\nDon't you dream about that promotion I mentioned last time!");
    }
  }
  
  private void OnTrapDeployed(int trapType) {
    // Send Analytics data
    UGS_Analytics.DeployTrapCustomEvent(trapType);
  }


  private void OnHeroDied(int numberLives, float xPos, float yPos, float zPos)
  {
    // Send Analytics data before ending the game
    UGS_Analytics.HeroDiedCustomEvent(numberLives, xPos, yPos, zPos);
    
    this._outOfBoundsTrigger.ToggleBounds(false);
    this.StartCoroutine(this.DisableOutOfBoundsForOneSecond());
  }

  private IEnumerator DisableOutOfBoundsForOneSecond() {
    yield return new WaitForSeconds(1f);

    if (this._hero == null)
      yield break;

    Vector2 newPos = (Vector2)this._hero.transform.position + this._playerRespawnOffset;
    this._player.MovePlayer(newPos);
    this._outOfBoundsTrigger.ToggleBounds(true);
  }

  private void GameWon() {
    this._player.PlayerController.DisablePlayerInput();
    this.OnHenWon?.Invoke("The Hero was stopped, good work Hendall!");
  }

  private void HeroReachedLevelEnd() {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    this.OnHenLost?.Invoke("The Hero managed to reach his goal and do heroic things.\nHendall, you failed me!");
  }

  private void HenOutOfBounds(float xPos, float yPos, float zPos) {
    // Send Analytics data before ending the game
    UGS_Analytics.PlayerDeathByBoundaryCustomEvent(CoinManager.Instance.Coins, xPos, yPos, zPos);
    
    this.HenDied("What are you doing here?\nI told you, you should always keep an eye on the Hero!");
  }

  private void HenDied(string message) {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);
    
    // To avoid calling the OnTriggerExit2D method in the OutOfBoundsTrigger class, don't destroy the player object 
    // Destroy(this._player.gameObject);
    this._player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    
    this.OnHenLost?.Invoke(message);
  }
}
