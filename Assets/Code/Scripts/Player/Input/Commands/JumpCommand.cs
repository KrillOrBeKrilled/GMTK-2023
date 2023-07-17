using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class JumpCommand : ICommand
    {
        private Pawn _controlledObject;

        public JumpCommand(Pawn controlledObject)
        {
            _controlledObject = controlledObject;
        }

        public void Execute()
        {
            _controlledObject.Jump();
        }
    }
}