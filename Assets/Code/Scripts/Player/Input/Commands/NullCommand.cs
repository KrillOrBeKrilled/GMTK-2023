using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    // Null Object Command, in case we want to set up buttons that do nothing (e.g. switching keybindings)
    public class NullCommand : ICommand
    {
        public void Execute()
        {
            
        }
    }
}