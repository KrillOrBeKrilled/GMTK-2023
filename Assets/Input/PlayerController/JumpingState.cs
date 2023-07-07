using System;
using UnityEngine;

namespace Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class JumpingState : IPlayerState
    {
        // TODO: Adjust multiplier values here
        private readonly float _jumpForce;

        public JumpingState(float jumpForce)
        {
            _jumpForce = jumpForce;
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player jumps...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(Transform player, Rigidbody2D rBody, float direction)
        {
            // Jump, flappy bird style allowing the player to jump as much as they want midair
            // To handle the check to be able to jump here or in the playerController?
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops jumping...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}
