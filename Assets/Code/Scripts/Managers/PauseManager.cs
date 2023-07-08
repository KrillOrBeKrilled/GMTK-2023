using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager: Singleton<PauseManager> {

  public UnityEvent<bool> OnPauseToggled;

  private bool _isPausable;
  private bool _isPaused;

  private PlayerInputActions _playerInputActions;

  public void SetIsPausable(bool isPausable) {
    this._isPausable = isPausable;
  }

  public void UnpauseGame() {
    Time.timeScale = 1f;
    this._isPaused = false;
    this.OnPauseToggled?.Invoke(this._isPaused);
  }

  protected override void Awake() {
    base.Awake();

    this._playerInputActions = new PlayerInputActions();
    this.OnPauseToggled = new UnityEvent<bool>();
  }

  private void OnEnable() {
    this._playerInputActions.Enable();
    this._playerInputActions.Pause.PauseAction.performed += this.TogglePausedState;
  }

  private void OnDisable() {
    this._playerInputActions.Disable();
    this._playerInputActions.Pause.PauseAction.performed += this.TogglePausedState;
  }

  private void TogglePausedState(InputAction.CallbackContext ctx) {
    if (!this._isPausable) {
      return;
    }

    if (this._isPaused) {
      this.UnpauseGame();
    } else {
      this.PauseGame();
    }
  }

  private void PauseGame() {
    Time.timeScale = 0f;
    this._isPaused = true;
    this.OnPauseToggled?.Invoke(this._isPaused);
  }
}
