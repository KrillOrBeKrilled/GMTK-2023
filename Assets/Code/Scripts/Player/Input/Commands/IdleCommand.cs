namespace Player
{
    public class IdleCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public IdleCommand(Pawn controlledObject)
        {
            this._controlledObject = controlledObject;
        }

        public void Execute()
        {
            this._controlledObject.StandIdle();
        }
    }
}
