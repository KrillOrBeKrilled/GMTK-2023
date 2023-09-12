namespace KrillOrBeKrilled.Common {
    public interface ITrapBuilder {
        /// <summary> Deals damage on the actor. </summary>
        public bool CanBuildTrap();

        public void SetGroundedStatus(bool isGrounded);
    }
}
