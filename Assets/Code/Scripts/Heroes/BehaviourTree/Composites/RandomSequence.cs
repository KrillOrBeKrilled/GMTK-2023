//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************
// RandomSequence
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// A composite node that acts as an AND logic gate, evaluating the children
    /// <see cref="Node">Nodes</see> in sequence and succeeding only if all
    /// <see cref="Node">Nodes</see> take on the <see cref="NodeStatus.SUCCESS"/> status.
    /// </summary>
    public class RandomSequence : Composite {
        
        public RandomSequence() : base() {}
        public RandomSequence(List<Node> children) : base(children) {}
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Iterates through and evaluates all child <see cref="Node">Nodes</see> at random unless one returns the
        /// <see cref="NodeStatus.FAILURE"/> or <see cref="NodeStatus.RUNNING"/> status and takes on that status. 
        /// </summary>
        /// <returns>
        /// The <see cref="NodeStatus.FAILURE"/> or <see cref="NodeStatus.RUNNING"/> states if any of the child
        /// <see cref="Node">Nodes</see> return such a status. If all children are evaluated without returning,
        /// returns the <see cref="NodeStatus.SUCCESS"/> status.
        /// </returns>
        internal override NodeStatus Evaluate() {
            var nodes = new List<Node>(this.Children);
            
            for (var i = 0; i < this.Children.Count; i++) {
                var node = nodes[Random.Range(0, nodes.Count)];
                nodes.Remove(node);
                
                switch (node.Evaluate()) {
                    case NodeStatus.FAILURE:
                        this.Status = NodeStatus.FAILURE;
                        return this.Status;
                    case NodeStatus.SUCCESS:
                        continue;
                    case NodeStatus.RUNNING:
                        this.Status = NodeStatus.RUNNING;
                        return this.Status;
                    default:
                        this.Status = NodeStatus.SUCCESS;
                        return this.Status;
                }
            }

            this.Status = NodeStatus.SUCCESS;
            return this.Status;
        }
        
        #endregion
    }
}
