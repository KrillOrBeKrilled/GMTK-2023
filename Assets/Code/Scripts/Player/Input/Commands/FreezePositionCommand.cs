namespace Player
{
    public class FreezeCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public FreezeCommand(Pawn controlledObject)
        {
            this._controlledObject = controlledObject;
        }

        public void Execute()
        {
            this._controlledObject.FreezePosition();
        }
    }
}
