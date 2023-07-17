using System.Collections.Generic;
using Code.Scripts.Player.Input.Commands;
using UnityEngine;

namespace Code.Scripts.Player.Input {
    /// <summary>
    /// Class used to declutter the PlayerController class, encapsulating the player's behaviour in each state
    /// to better reason about correctness and pinpoint bugs easily, plus specialize behaviours to specific states. 
    /// </summary>
    public interface IPlayerState
    {
        public float GetMovementSpeed();
        
        // Methods OnEnter and OnExit that can be extended for future purposes to encapsulate rendering, sound, and logic
        // that are associated with each state.
        // TODO: Consider what we need when switching states...reference to old state, to play audio, etc.
        public void OnEnter(IPlayerState prevState) {}
        public void OnExit(IPlayerState newState) {}

        /// <summary>
        /// Adjust the player movement speed and actions depending on the current state of the player.
        /// Idle: Player stands still, can transition to any other state in this state
        /// Moving: Player moves in this state in a given direction along the x-axis
        /// Jumping: Player can jump as many times in the air
        /// Deploying: Player is placing a trap on a given tile space
        /// </summary>
        abstract void Act(PlayerController playerController, float direction, List<ICommand> prevCommands);
    }
}