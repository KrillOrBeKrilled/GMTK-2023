//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Sequence
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A composite node that acts as an AND logic gate, evaluating the children
    /// <see cref="Node">Nodes</see> in sequence and succeeding only if all
    /// <see cref="Node">Nodes</see> take on the <see cref="NodeStatus.SUCCESS"/> status.
    /// </summary>
    public class Sequence : Composite {
        
        public Sequence() : base() {}
        public Sequence(List<Node> children) : base(children) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Iterates through and evaluates all child <see cref="Node">Nodes</see> unless one returns the
        /// <see cref="NodeStatus.FAILURE"/> or <see cref="NodeStatus.RUNNING"/> status and takes on that status. 
        /// </summary>
        /// <returns>
        /// The <see cref="NodeStatus.FAILURE"/> or <see cref="NodeStatus.RUNNING"/> states if any of the child
        /// <see cref="Node">Nodes</see> return such a status. If all children are evaluated without returning,
        /// returns the <see cref="NodeStatus.SUCCESS"/> status.
        /// </returns>
        internal override NodeStatus Evaluate() {
            foreach (var node in Children) {
                switch (node.Evaluate()) {
                    case NodeStatus.FAILURE:
                        Status = NodeStatus.FAILURE;
                        return Status;
                    case NodeStatus.SUCCESS:
                        continue;
                    case NodeStatus.RUNNING:
                        Status = NodeStatus.RUNNING;
                        return Status;
                    default:
                        Status = NodeStatus.SUCCESS;
                        return Status;
                }
            }

            Status = NodeStatus.SUCCESS;
            return Status;
        }
        
        #endregion
    }
}
