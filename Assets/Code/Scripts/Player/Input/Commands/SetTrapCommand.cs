namespace Player
{
    public class SetTrapCommand : ICommand
    {
        private readonly Pawn _controlledObject;
        private readonly int _trapIndex;

        public SetTrapCommand(Pawn controlledObject, int trapIndex)
        {
            this._controlledObject = controlledObject;
            this._trapIndex = trapIndex;
        }

        public int GetTrapIndex()
        {
            return this._trapIndex;
        }


        public void Execute()
        {
            this._controlledObject.ChangeTrap(this._trapIndex);
        }
    }
}
