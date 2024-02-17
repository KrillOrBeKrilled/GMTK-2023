using DG.Tweening;
using KrillOrBeKrilled.Player.Commands;
using State = KrillOrBeKrilled.Player.PlayerCharacter.State;
using UnityEngine;

//*******************************************************************************************
// JumpingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player jumping state.
    /// </summary>
    public class GlidingState : IPlayerState {
        private readonly PlayerCharacter _player;

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
        public GlidingState(PlayerCharacter player) {
            this._player = player;
        }

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            if (this._player.IsGrounded || !jumpPressed) {
                bool isMoving = !Mathf.Approximately(moveInput, 0f);
                this._player.ChangeState(isMoving ? State.Moving : State.Idle);
                return;
            }

            // Create command and execute it
            var command = new GlideCommand(this._player, moveInput, this._glideSpeedMultiplier);
            this._player.ExecuteCommand(command);
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
