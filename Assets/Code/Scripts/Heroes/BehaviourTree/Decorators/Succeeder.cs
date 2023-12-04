//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//
// TIP: "These are useful in cases where you want to process a branch of a tree where a
// failure is expected or anticipated, but you don’t want to abandon processing of a
// sequence that branch sits on." ->
// https://www.gamedeveloper.com/programming/behavior-trees-for-ai-how-they-work#close-modal
//-------------------------------------------------------------------------------------------

//*******************************************************************************************
// Succeeder
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A decorator node that always returns <see cref="NodeStatus.SUCCESS"/>.
    /// </summary>
    /// <remarks> To get a Failer, attach an Inverter to this Succeeder. </remarks>
    public class Succeeder : Decorator {
        
        public Succeeder() : base() {}
        public Succeeder(Node child) : base(child) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Evaluates success regardless of the child status. 
        /// </summary>
        /// <returns>
        /// <see cref="NodeStatus.SUCCESS"/> regardless of whether the child node evaluates to
        /// <see cref="NodeStatus.SUCCESS"/> or <see cref="NodeStatus.FAILURE"/>. If the child
        /// node's status is <see cref="NodeStatus.RUNNING"/>, takes on this status.
        /// </returns>
        internal override NodeStatus Evaluate() {
            // Validate child node
            if (!this.HasChildren) {
                return NodeStatus.FAILURE;
            }

            if (this.Child.Evaluate() == NodeStatus.RUNNING) {
                this.Status = NodeStatus.RUNNING;
                return this.Status;
            }
            
            this.Status = NodeStatus.SUCCESS;
            return this.Status;
        }
        
        #endregion
    }
}