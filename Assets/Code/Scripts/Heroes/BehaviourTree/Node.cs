//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using System.Collections.Generic;

//*******************************************************************************************
// Node
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// Represents the execution state of the node computed by evaluation.
    /// <list Type="bullet">
    /// <item> <b>RUNNING</b>: The node is currently running. </item>
    /// <item> <b>SUCCESS</b>: The node evaluates to "true", completing its action. </item>
    /// <item> <b>FAILURE</b>: The node evaluates to "false", forfeiting its action. </item>
    /// </list>
    /// </summary>
    public enum NodeStatus {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    
    /// <summary>
    /// Makes up a Behaviour Tree as units encapsulating composite Type selectors, sequences, etc., decorator Type
    /// inverters, succeeders, etc., and leaves.
    /// </summary>
    public class Node {
        /// The current state of this node to communicate to other composite and decorator Nodes during evaluation.
        protected NodeStatus Status;
        
        /// Reference to the parent Node for ease of access and the sharing of data through composite and decorator
        /// nodes.
        internal Node Parent;

        // Allows for the sharing of data between Nodes in key-value pairs.
        private readonly Dictionary<string, object> _dataContext = new Dictionary<string, object>();
        
        internal Node() {
            Parent = null;
        }

        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Attempts to recursively delete the data associated with the provided key from this node and all parent
        /// nodes of the upper levels. 
        /// </summary>
        /// <param name="key"> The unique name to access the target data. </param>
        /// <returns> If the key-value pair of data has been successfully removed. </returns>
        internal bool ClearData(string key) {
            if (this._dataContext.ContainsKey(key)) {
                this._dataContext.Remove(key);
                return true;
            }
            
            var currNode = this.Parent;
            while (currNode != null) {
                var cleared = currNode.ClearData(key);
                
                if (cleared) {
                    return true;
                }

                currNode = currNode.Parent;
            }

            return false;
        }

        // Each derived Node class can override this Evaluate method to specify different logic to be executed in the 
        // Behaviour Tree.
        /// <summary>
        /// Executes the action associated with this node.
        /// </summary>
        /// <returns> The status of this <see cref="Node"/> after evaluation. </returns>
        /// <remarks> Executed on Update. </remarks>
        internal virtual NodeStatus Evaluate() => NodeStatus.FAILURE;

        /// <summary>
        /// Attempts to recursively retrieve the data associated with the provided key from this node and all parent
        /// nodes of the upper levels. 
        /// </summary>
        /// <param name="key"> The unique name to access the target data. </param>
        /// <returns>
        /// The data associated with the provided key. If no such key exists within any of the searched
        /// nodes, returns <b>null</b>.
        /// </returns>
        internal object GetData(string key) {
            if (this._dataContext.TryGetValue(key, out var value)) {
                return value;
            }
            
            var currNode = this.Parent;
            while (currNode != null) {
                value = currNode.GetData(key);
                
                if (value != null) {
                    return value;
                }

                currNode = currNode.Parent;
            }

            return null;
        }
        
        /// <summary>
        /// Adds data or updates the associated data if the provided key already exists to the <see cref="Node"/>
        /// bookkeeping structures.
        /// </summary>
        /// <param name="key"> The unique name to access the target data. </param>
        /// <param name="value"> The new value to overwrite the data associated with the provided key. </param>
        internal void SetData(string key, object value) {
            this._dataContext[key] = value;
        }

        #endregion
    }
}
