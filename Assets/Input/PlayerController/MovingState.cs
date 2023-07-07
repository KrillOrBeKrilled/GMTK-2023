using System;
using UnityEngine;

namespace Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class MovingState : IPlayerState
    {
        // TODO: Adjust multiplier values here
        private readonly float _stateSpeed;
        private readonly float _stateSpeedBlendDuration;

        private float _currentSpeed, _t;

        public MovingState(float stateSpeed, float stateSpeedBlendDuration)
        {
            _stateSpeedBlendDuration = stateSpeedBlendDuration;
            _currentSpeed = stateSpeed;
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(Transform player, Rigidbody2D rBody, float direction)
        {
            
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}