//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

//*******************************************************************************************
// Inverter
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A decorator node that returns the negated status of its child <see cref="Node"/>.
    /// </summary>
    /// <remarks> Can be used as conditional tests. </remarks>
    public class Inverter : Decorator {
        
        public Inverter() : base() {}
        public Inverter(Node child) : base(child) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Evaluates the child <see cref="Node"/> and negates the returned status. 
        /// </summary>
        /// <returns>
        /// The negated status of the child node. If the child node has a <see cref="NodeStatus.RUNNING"/> state,
        /// returns <see cref="NodeStatus.RUNNING"/>.
        /// </returns>
        internal override NodeStatus Evaluate() {
            // Validate child node
            if (!this.HasChildren) {
                return NodeStatus.FAILURE;
            }
            
            switch (Child.Evaluate()) {
                case NodeStatus.FAILURE:
                    this.Status = NodeStatus.SUCCESS;
                    return this.Status;
                case NodeStatus.SUCCESS:
                    this.Status = NodeStatus.FAILURE;
                    return this.Status;
                case NodeStatus.RUNNING:
                    this.Status = NodeStatus.RUNNING;
                    return this.Status;
                default:
                    this.Status = NodeStatus.FAILURE;
                    return this.Status;
            }
        }
        
        #endregion
    }
}