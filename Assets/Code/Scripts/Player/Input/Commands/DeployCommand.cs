namespace Player
{
    public class DeployCommand : ICommand
    {
        private readonly Pawn _controlledObject;

        public DeployCommand(Pawn controlledObject)
        {
            this._controlledObject = controlledObject;
        }

        public void Execute()
        {
            this._controlledObject.DeployTrap();
        }
    }
}
