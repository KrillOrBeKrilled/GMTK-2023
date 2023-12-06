//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Parallel
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A composite node that evaluates all children <see cref="Node">Nodes</see> in
    /// parallel and fails only if all <see cref="Node">Nodes</see> take on the
    /// <see cref="NodeStatus.FAILURE"/> status.
    /// </summary>
    public class Parallel : Composite {
        
        public Parallel() : base() {}
        public Parallel(List<Node> children) : base(children) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Iterates through and evaluates all child <see cref="Node">Nodes</see> in parallel. 
        /// </summary>
        /// <returns>
        /// The <see cref="NodeStatus.FAILURE"/> state if all of the child <see cref="Node">Nodes</see> return such a
        /// status. If any child is currently <see cref="NodeStatus.RUNNING"/>, returns the same status. Otherwise,
        /// returns the <see cref="NodeStatus.SUCCESS"/> status.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var childIsRunning = false;
            var failedCount = 0;
            
            foreach (var node in this.Children) {
                switch (node.Evaluate()) {
                    case NodeStatus.FAILURE:
                        failedCount++;
                        continue;
                    case NodeStatus.SUCCESS:
                        continue;
                    case NodeStatus.RUNNING:
                        childIsRunning = true;
                        continue;
                    default:
                        Status = NodeStatus.SUCCESS;
                        return Status;
                }
            }

            if (failedCount == this.Children.Count) {
                this.Status = NodeStatus.FAILURE;
            } else {
                this.Status = childIsRunning ? NodeStatus.RUNNING : NodeStatus.SUCCESS;
            }
            
            return this.Status;
        }
        
        #endregion
    }
}
