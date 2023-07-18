using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class DeployCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public DeployCommand(Pawn controlledObject)
        {
            _controlledObject = controlledObject;
        }

        public void Execute()
        {
            _controlledObject.DeployTrap();
        }
    }
}