namespace Input {
    public class PlayerInputController : Singleton<PlayerInputController> {
        /// <summary>
        /// Class to handle character controls
        /// TODO: Make note of any music plugins we need here...
        /// </summary>
        // -------------- Input System ---------------
        public PlayerInputActions PlayerInputActions { get; private set; }

        protected override void Awake() {
            base.Awake();
            this.PlayerInputActions = new PlayerInputActions();
            this.PlayerInputActions.Enable();
        }
        
        // To help with UI stuff when disabling and enabling controls
        public void DisablePlayerControls()
        {
            this.PlayerInputActions.Player.Disable();
        }
        
        public void EnablePlayerControls()
        {
            this.PlayerInputActions.Player.Enable();
        }
        
        public void DisableUIControls()
        {
            this.PlayerInputActions.Pause.Disable();
        }
        
        public void EnableUIControls()
        {
            this.PlayerInputActions.Pause.Enable();
        }
    }
}
