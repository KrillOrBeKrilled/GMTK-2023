using Code.Scripts.UI;
using Input;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class GameManager : Singleton<GameManager> {
  [SerializeField] private GameUI _gameUI;
  [SerializeField] private Player _player;
  [SerializeField] private Hero _hero;
  [SerializeField] private EndgameTarget _endgameTarget;
  [SerializeField] private Vector2 _playerRespawnOffset;
  [SerializeField] private DialogueRunner _dialogueRunner;

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
    CoinManager.Instance.StartCoinEarning();
  }

  private void Start() {
    // Setup
    this._gameUI.Initialize(this, this._player);

    _hero.ResetHero();

    this._endgameTarget.OnHeroReachedEndgameTarget.AddListener(this.HeroReachedLevelEnd);
    this._player.PlayerController.OnPlayerStateChanged.AddListener(this.OnPlayerStateChanged);
    this._player.PlayerController.OnSelectedTrapIndexChanged.AddListener(this.SelectedTrapIndexChanged);
    this._player.PlayerController.OnTrapDeployed.AddListener(this.OnTrapDeployed);
    this._player.PlayerController.OnSkipDialoguePerformed.AddListener(this.OnSkipDialoguePerformed);
    this._hero.OnGameOver.AddListener(this.GameWon);
    this._hero.OnHeroDied.AddListener(this.OnHeroDied);
    this._hero.HeroMovement.OnHeroIsStuck.AddListener(this.OnHeroIsStuck);

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

  private void SelectedTrapIndexChanged(int trapIndex)
  {
    var isAffordable = _player.PlayerController.GetTrapCost() >= CoinManager.Instance.Coins;

    // Send Analytics data
    UGS_Analytics.SwitchTrapCustomEvent(trapIndex, isAffordable);
  }

  private void OnTrapDeployed(int trapIndex) {
    // Send Analytics data
    UGS_Analytics.DeployTrapCustomEvent(trapIndex);
  }


  private void OnHeroDied(int numberLives, float xPos, float yPos, float zPos)
  {
    // Send Analytics data before ending the game
    UGS_Analytics.HeroDiedCustomEvent(numberLives, xPos, yPos, zPos);

    this.StartCoroutine(this.DisableOutOfBoundsForOneSecond());
  }

  private void OnHeroIsStuck(float xPos, float yPos, float zPos)
  {
    // Send Analytics data
    UGS_Analytics.HeroIsStuckCustomEvent(xPos, yPos, zPos);
  }

  private IEnumerator DisableOutOfBoundsForOneSecond() {
    yield return new WaitForSeconds(1f);

    if (this._hero == null)
      yield break;

    Vector2 newPos = (Vector2)this._hero.transform.position + this._playerRespawnOffset;
    this._player.MovePlayer(newPos);
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

  private void HenDied(string message) {
    this._player.PlayerController.DisablePlayerInput();
    this._hero.HeroMovement.ToggleMoving(false);

    // To avoid calling the OnTriggerExit2D method in the OutOfBoundsTrigger class, don't destroy the player object
    // Destroy(this._player.gameObject);
    this._player.gameObject.GetComponent<SpriteRenderer>().enabled = false;

    this.OnHenLost?.Invoke(message);
  }

  private void OnSkipDialoguePerformed(InputAction.CallbackContext _) {
    this._dialogueRunner.Stop();
  }
}
