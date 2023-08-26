namespace Player
{
    public class MoveCommand : ICommand
    {
        private readonly Pawn _controlledObject;
        private readonly float _inputDirection;

        public MoveCommand(Pawn controlledObject, float inputDirection)
        {
            this._controlledObject = controlledObject;
            this._inputDirection = inputDirection;
        }

        public float GetDirection()
        {
            return this._inputDirection;
        }

        public void Execute()
        {
            this._controlledObject.Move(this._inputDirection);
        }
    }
}
