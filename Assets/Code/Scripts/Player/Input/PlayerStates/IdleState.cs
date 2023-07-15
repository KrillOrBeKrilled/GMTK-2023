using System;
using UnityEngine;

namespace Input {
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
        
        public void Act(Rigidbody2D rBody, float direction, Action enterIdle)
        {
            rBody.velocity = new Vector2(0f, rBody.velocity.y);
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops idling...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}