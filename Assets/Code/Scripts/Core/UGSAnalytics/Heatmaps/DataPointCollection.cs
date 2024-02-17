using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//*******************************************************************************************
// DataPointCollection
//*******************************************************************************************
namespace KrillOrBeKrilled.Core.UGSAnalytics.Heatmaps {
    /// <summary>
    /// Manages a set of <see cref="DataPoint">DataPoints</see>, updating the list and
    /// rostered <see cref="DataPoint">DataPoints</see> any time the scene is updated in
    /// the Unity Editor.
    /// </summary>
    [ExecuteInEditMode]
    public class DataPointCollection : MonoBehaviour {
        [Tooltip("Stores all the DataPoints currently registered with the heatmap.")]
        [SerializeField] internal List<DataPoint> DataPoints;
        
        [Tooltip("All DataPoints that fall within this range contribute to the point concentration calculations.")]
        [SerializeField] internal float PointTargetRadius;
        
        [Tooltip("Visualizes the PointTargetRadius.")]
        [SerializeField] internal bool VisualizeTargetRadius;

        [Tooltip("The maximum value of point concentration to scale the DataPoint colors and scales.")]
        [SerializeField] internal int MaxPointConcentrationSpectrum;
        
        [Tooltip("Color scheme of the 'heat', starting from the SpectrumBorder (cold) to the SpectrumCenter (hot)")]
        [SerializeField] internal Color SpectrumBorder, SpectrumCenter;

        [Tooltip("Scale scheme, ranging between the PointBaseScale to the MaxScaleSpectrum")]
        [SerializeField] internal Vector3 PointBaseScale, MaxScaleSpectrum;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void OnEnable() {
            #if UNITY_EDITOR
                EditorApplication.hierarchyChanged += this.OnHierarchyChanged;
            #endif
        }

        private void OnDisable() {
            #if UNITY_EDITOR
                EditorApplication.hierarchyChanged -= this.OnHierarchyChanged;
            #endif
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Destroys all the <see cref="DataPoint">DataPoints</see> recorded in <see cref="DataPoints"/>.
        /// </summary>
        /// <remarks> Invoked by <see cref="Heatmap"/> to clear the entire heatmap. </remarks>
        internal void ClearPoints() {
            foreach (var point in this.DataPoints) {
                DestroyImmediate(point.gameObject);
            }
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Updates the list of <see cref="DataPoint">DataPoints</see> and each <see cref="DataPoint"/>.
        /// </summary>
        /// <remarks> Invoked every time the scene is updated in the Unity Editor. </remarks>
        private void OnHierarchyChanged() {
            this.DataPoints.Clear();

            // Update the list of data points as well as each point
            var points = FindObjectsOfType<DataPoint>();
            foreach (var point in points) {
                this.DataPoints.Add(point);
                point.UpdatePoint(this);
            }
        }
        
        #endregion
    }
}
