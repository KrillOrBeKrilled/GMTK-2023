using System;
using System.Collections.Generic;
using Code.Scripts.Player.Input.Commands;
using UnityEngine;

namespace Code.Scripts.Player.Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class IdleState : IPlayerState
    {
        public IdleState() {}
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player walks...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(PlayerController playerController, float direction)
        {
            // Create command and execute it
            var command = new IdleCommand(playerController);
            playerController.ExecuteCommand(command);
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops idling...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}