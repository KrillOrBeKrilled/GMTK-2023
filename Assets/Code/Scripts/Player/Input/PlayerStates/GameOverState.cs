using System;
using System.Collections.Generic;
using Code.Scripts.Player.Input.Commands;
using UnityEngine;

namespace Code.Scripts.Player.Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class GameOverState : IPlayerState
    {
        public GameOverState()
        {
            
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the game ends...what should happen? music? visual animations?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(PlayerController playerController, float direction, List<ICommand> prevCommands)
        {
            // The game is over! Maybe the player shouldn't do anything anymore and some UI opens or something
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the game ends...what should happen? music? visual animations?
        }
    }
}