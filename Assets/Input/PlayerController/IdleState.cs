using System;
using UnityEngine;

namespace Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class IdleState : IPlayerState
    {
        // TODO: Adjust multiplier values here
        private readonly float _stateSpeed;
        private readonly float _stateSpeedBlendDuration;

        private float _currentSpeed, _t;

        public IdleState(float stateSpeed, float stateSpeedBlendDuration)
        {
            _stateSpeedBlendDuration = stateSpeedBlendDuration;
            _currentSpeed = stateSpeed;
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player walks...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(Transform player, Rigidbody2D rBody, float direction)
        {
            // Don't move when the player is idle
            // Probably just rely on Unity's built-in physics system for handling the jumping and falling
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops idling...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}