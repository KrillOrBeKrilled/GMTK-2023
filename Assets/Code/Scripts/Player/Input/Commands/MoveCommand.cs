using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Pawn _controlledObject;
        private readonly float _inputDirection;
        
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
