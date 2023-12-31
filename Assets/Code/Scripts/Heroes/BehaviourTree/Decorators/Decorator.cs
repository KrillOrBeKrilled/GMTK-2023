//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Decorator
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// Makes up a Behaviour Tree as flow control units that evaluate a single child.
    /// Use cases include inverters, succeeders, repeaters, etc.
    /// </summary>
    public class Decorator : Node {
        /// The child associated with this Node.
        protected Node Child;
        
        /// If this node has links to any child nodes.
        protected readonly bool HasChildren;
        
        internal Decorator() : base() {}
        
        /// <summary>
        /// Creates an edge between this node and the provided <see cref="Node"/>.
        /// </summary>
        /// <param name="child"> The <see cref="Node"/> to link to this <see cref="Node"/> as children. </param>
        internal Decorator(Node child) {
            AddChild(child);
            this.HasChildren = true;
        }
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Creates an edge between this node and its new child.
        /// </summary>
        /// <param name="node"> The node to be added as a new child to this <see cref="Node"/>. </param>
        private void AddChild(Node node) {
            node.Parent = this;
            this.Child = node;
        }
        
        #endregion
    }
}
