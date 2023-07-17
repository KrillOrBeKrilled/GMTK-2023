using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Player.Input.Commands
{
    public class SetTrapCommand : ICommand
    {
        private Pawn _controlledObject;
        private int _trapIndex;

        public SetTrapCommand(Pawn controlledObject, int trapIndex)
        {
            _controlledObject = controlledObject;
            _trapIndex = trapIndex;
        }

        public void Execute()
        {
            _controlledObject.ChangeTrap(_trapIndex);
        }
    }
}
