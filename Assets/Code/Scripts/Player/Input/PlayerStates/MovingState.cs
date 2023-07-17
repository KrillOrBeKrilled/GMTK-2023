using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Player.Input.Commands;
using UnityEngine;

namespace Code.Scripts.Player.Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class MovingState : IPlayerState
    {
        // TODO: Adjust multiplier values here
        private readonly float _stateSpeed;

        public MovingState(float stateSpeed)
        {
            _stateSpeed = stateSpeed;
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return _stateSpeed;
        }
        
        public void Act(PlayerController playerController, float direction, List<ICommand> prevCommands)
        {
            // Create command and execute it
            // Pass by reference in the list; need to create a new command each time to add to the list (Is there a
            // better way of doing this, memory-wise?)
            var command = new MoveCommand(playerController, direction);
            playerController.ExecuteCommand(command);
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}