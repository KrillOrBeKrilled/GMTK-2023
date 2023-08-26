namespace Player
{
    public class JumpCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public JumpCommand(Pawn controlledObject)
        {
            this._controlledObject = controlledObject;
        }

        public void Execute()
        {
            this._controlledObject.Jump();
        }
    }
}
