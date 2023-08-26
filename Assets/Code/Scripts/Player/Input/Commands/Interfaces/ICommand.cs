namespace Player
{
    public interface ICommand
    {
        public void Execute();

        public float GetDirection()
        {
            return 0f;
        }

        public int GetTrapIndex()
        {
            return 0;
        }
    }
}
