namespace Player
{
    // Null Object Command, in case we want to set up buttons that do nothing (e.g. switching keybindings)
    public class NullCommand : ICommand
    {
        public void Execute()
        {

        }
    }
}
