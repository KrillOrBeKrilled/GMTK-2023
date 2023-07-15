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
        
        public void Act(Rigidbody2D rBody, float direction, Action enterIdle)
        {
            rBody.velocity = new Vector2(direction * _stateSpeed, rBody.velocity.y);
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}