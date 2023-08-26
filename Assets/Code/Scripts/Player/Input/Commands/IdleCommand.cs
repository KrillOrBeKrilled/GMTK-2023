using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class IdleCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public IdleCommand(Pawn controlledObject)
        {
            _controlledObject = controlledObject;
        }

        public void Execute()
        {
            _controlledObject.StandIdle();
        }
    }
}