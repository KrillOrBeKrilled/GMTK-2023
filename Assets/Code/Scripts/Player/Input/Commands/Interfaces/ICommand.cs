using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public interface ICommand
    {
        public void Execute();

        public float GetDirection()
        {
            return 0f;
        }
        
        public int GetTrapIndex()
        {
            return 0;
        }
    }
}
