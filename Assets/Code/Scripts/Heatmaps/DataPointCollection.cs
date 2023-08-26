using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Heatmaps {
    [ExecuteInEditMode]
    public class DataPointCollection : MonoBehaviour
    {
        public List<DataPoint> DataPoints;
        public float PointTargetRadius;
        public bool VisualizeTargetRadius;

        public int MaxPointConcentrationSpectrum;
        public Color SpectrumBorder, SpectrumCenter;

        public Vector3 PointBaseScale;
        public Vector3 MaxScaleSpectrum;

        public void ClearPoints()
        {
            foreach (var point in this.DataPoints)
            {
                DestroyImmediate(point.gameObject);
            }
        }

        private void OnHierarchyChanged()
        {
            this.DataPoints.Clear();

            // Update the list of data points as well as each point
            var points = FindObjectsOfType<DataPoint>();
            foreach (var point in points)
            {
                this.DataPoints.Add(point);
                point.UpdatePoint(this);
            }
        }

        private void OnEnable()
        {
        #if UNITY_EDITOR
            EditorApplication.hierarchyChanged += this.OnHierarchyChanged;
        #endif
        }

        private void OnDisable()
        {
        #if UNITY_EDITOR
            EditorApplication.hierarchyChanged -= this.OnHierarchyChanged;
        #endif
        }
    }
}
