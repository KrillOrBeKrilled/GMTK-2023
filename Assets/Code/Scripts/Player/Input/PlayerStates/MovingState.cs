namespace Player {
    /// <summary>
    /// TODO: Make note of any music plugins we need here...
    /// </summary>
    public class MovingState : IPlayerState
    {
        // TODO: Adjust multiplier values here
        private readonly float _stateSpeed;

        public MovingState(float stateSpeed)
        {
            this._stateSpeed = stateSpeed;
        }

        public void OnEnter(IPlayerState prevState)
        {
            // TODO: When the Player moves...what should happen? music? visual animations? Does it matter from which
            // state?
        }

        public float GetMovementSpeed()
        {
            return this._stateSpeed;
        }

        public void Act(PlayerController playerController, float direction)
        {
            // Create command and execute it
            var command = new MoveCommand(playerController, direction);
            playerController.ExecuteCommand(command);
        }

        public void OnExit(IPlayerState newState)
        {
            // TODO: When the Player stops moving...what should happen? music? visual animations? Does
            // it matter to which state?
        }
    }
}