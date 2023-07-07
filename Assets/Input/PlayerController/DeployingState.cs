using System;
using UnityEngine;

namespace Input {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class DeployingState : IPlayerState
    {
        public DeployingState()
        {
            
        }
        
        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player deploys a trap...what should happen? music? visual animations? Does it matter from
            // which state?
        }

        public float GetMovementSpeed()
        {
            return 0f;
        }
        
        public void Act(Rigidbody2D rBody, float direction, Action enterIdle)
        {
            // Deploy a trap at the highlighted/selected tile(s)
        }
        
        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops deploying...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}