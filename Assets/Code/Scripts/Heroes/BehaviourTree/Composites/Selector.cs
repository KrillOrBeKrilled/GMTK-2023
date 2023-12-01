//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Selector
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A composite node that acts as an OR logic gate, evaluating the children
    /// <see cref="Node">Nodes</see> in sequence and succeeding if at least one
    /// <see cref="Node"/> takes on the <see cref="NodeStatus.SUCCESS"/> status.
    /// </summary>
    public class Selector : Composite {
        
        public Selector() : base() {}
        public Selector(List<Node> children) : base(children) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Iterates through and evaluates all child <see cref="Node">Nodes</see> unless one returns the
        /// <see cref="NodeStatus.SUCCESS"/> or <see cref="NodeStatus.RUNNING"/> status and takes on that status. 
        /// </summary>
        /// <returns>
        /// The <see cref="NodeStatus.SUCCESS"/> or <see cref="NodeStatus.RUNNING"/> states if any of the child
        /// <see cref="Node">Nodes</see> return such a status. If all children are evaluated without returning,
        /// returns the <see cref="NodeStatus.FAILURE"/> status.
        /// </returns>
        internal override NodeStatus Evaluate() {
            foreach (var node in this.Children) {
                switch (node.Evaluate()) {
                    case NodeStatus.FAILURE:
                        continue;
                    case NodeStatus.SUCCESS:
                        this.Status = NodeStatus.SUCCESS;
                        return this.Status;
                    case NodeStatus.RUNNING:
                        this.Status = NodeStatus.RUNNING;
                        return this.Status;
                    default:
                        continue;
                }
            }

            this.Status = NodeStatus.FAILURE;
            return this.Status;
        }
        
        #endregion
    }
}