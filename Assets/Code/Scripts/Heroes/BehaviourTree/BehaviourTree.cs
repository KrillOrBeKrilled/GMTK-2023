//-------------------------------------------------------------------------------------------
// DISCLAIMER: The tools included in this namespace have been adapted from
// https://www.youtube.com/watch?v=aR6wt5BlE-E
//-------------------------------------------------------------------------------------------

using UnityEngine;

//*******************************************************************************************
// BehaviourTree
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.BehaviourTree {
    /// <summary>
    /// Abstract class that sets up and continuously updates the nodes that make up this
    /// tree. The tree setup is abstracted to invite flexibility with the reuse and mixing
    /// of composites, decorators, and leaves in relation to each other.
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour {
        private Node _root;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        protected void Start() {
            this._root = SetupTree();
        }
        
        private void Update() {
            this._root?.Evaluate();
        }
        
        #endregion
        
        //========================================
        // Protected Methods
        //========================================
        
        #region Protected Methods

        /// <summary>
        /// Defines this Behaviour Tree's structure as a combination of composite, decorator, and leaf nodes.
        /// </summary>
        /// <returns> The root <see cref="Node"/> of the defined structure. </returns>
        protected abstract Node SetupTree();
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods

        /// <summary>
        /// Upserts the data in the root <see cref="Node"/> of this Behaviour Tree if such a root exists.
        /// </summary>
        internal void UpdateData(string key, object value) {
            this._root?.SetData(key, value);
        }
        
        #endregion
    }
}
