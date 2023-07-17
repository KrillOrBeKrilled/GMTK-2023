using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class MoveCommand : ICommand
    {
        private Pawn _controlledObject;
        private float _inputDirection;
        
        public MoveCommand(Pawn controlledObject, float inputDirection)
        {
            _controlledObject = controlledObject;
            _inputDirection = inputDirection;
        }

        public void Execute()
        {
            _controlledObject.Move(_inputDirection);
        }
    }
}
