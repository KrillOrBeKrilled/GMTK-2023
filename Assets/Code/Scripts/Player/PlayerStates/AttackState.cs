using KrillOrBeKrilled.Player.Commands;
using State = KrillOrBeKrilled.Player.PlayerCharacter.State;
using UnityEngine;

//*******************************************************************************************
// AttackingState
//*******************************************************************************************
namespace KrillOrBeKrilled.Player.PlayerStates {
    /// <summary>
    /// Implements <see cref="IPlayerState"/> to encapsulate logic, visuals, and sounds
    /// associated with the player jumping state.
    /// </summary>
    public class AttackingState : IPlayerState {
        private readonly PlayerCharacter _player;
        private readonly int _attackKey = Animator.StringToHash("is_attacking");

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Constructor to set bookkeeping data related to this state to act on the player.
        /// </summary>
        public AttackingState(PlayerCharacter player) {
            this._player = player;
        }

        public void Act(float moveInput, bool jumpPressed, bool jumpPressedThisFrame) {
            // Check if need to change state
            if (jumpPressedThisFrame) {
                State nextState = this._player.IsGrounded ? State.Jumping : State.Gliding;
                this._player.ChangeState(nextState);
                return;
            }

            if (!Mathf.Approximately(moveInput, 0f)) {
                this._player.ChangeState(State.Moving);
            }
            
            // Prevent the hero from pushing the hen while attacking
            var command = new IdleCommand(this._player);
            this._player.ExecuteCommand(command);
        }

        public void OnEnter(IPlayerState prevState) {
            this._player.Animator.SetTrigger(_attackKey);
        }

        public void OnExit(IPlayerState newState) {

        }

        #endregion
    }
}
