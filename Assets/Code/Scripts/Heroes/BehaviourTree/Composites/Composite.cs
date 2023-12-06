//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Composite
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// Makes up a Behaviour Tree as flow control units that evaluate over multiple
    /// children. Use cases include selectors, sequences, parallels, etc.
    /// </summary>
    public class Composite : Node {
        /// The children associated with this Node.
        protected readonly List<Node> Children = new List<Node>();
        
        internal Composite() : base() {}
        
        /// <summary>
        /// Creates edges linked to this node for all provided <see cref="Node">Nodes</see>.
        /// </summary>
        /// <param name="children">
        /// A list of <see cref="Node">Nodes</see> to link to this <see cref="Node"/> as children.
        /// </param>
        internal Composite(List<Node> children) {
            foreach (var child in children) {
                AddChild(child);
            }
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
            this.Children.Add(node);
        }
        
        #endregion
    }
}
