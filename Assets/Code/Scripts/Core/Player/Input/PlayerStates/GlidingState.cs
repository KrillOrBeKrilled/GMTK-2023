using DG.Tweening;
using KrillOrBeKrilled.Core.Commands;
using State = KrillOrBeKrilled.Core.Player.PlayerController.State;
using UnityEngine;

//*******************************************************************************************
// JumpingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.Player {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player jumping state.
    /// </summary>
    public class GlidingState : IPlayerState {
        private readonly PlayerController _playerController;

        private float _glideSpeedMultiplier = 1f;
        private const float GlideSpeedMultiplierStart = 1f;
        private const float GlideSpeedMultiplierEnd = 1.3f;
        private const float GlideMultiplierTweenDuration = 1f;

        private Tween _glideTween;

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        public GlidingState(PlayerController playerController) {
            this._playerController = playerController;
        }

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            if (this._playerController.IsGrounded || !jumpPressed) {
                bool isMoving = !Mathf.Approximately(moveInput, 0f);
                this._playerController.ChangeState(isMoving ? State.Moving : State.Idle);
                return;
            }

            // Create command and execute it
            var command = new GlideCommand(this._playerController, moveInput, this._glideSpeedMultiplier);
            this._playerController.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            this._glideTween = DOVirtual
                .Float(GlideSpeedMultiplierStart, GlideSpeedMultiplierEnd, GlideMultiplierTweenDuration,
                    newMultiplier => { this._glideSpeedMultiplier = newMultiplier; })
                .SetEase(Ease.OutExpo);
        }

        public void OnExit(IPlayerState newState) {
            this._glideTween.Kill();
        }

        #endregion
    }
}
