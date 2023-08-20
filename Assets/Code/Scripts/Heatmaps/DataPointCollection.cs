using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        foreach (var point in DataPoints)
        {
            DestroyImmediate(point.gameObject);
        }
    }
    
    private void OnHierarchyChanged()
    {
        DataPoints.Clear();
        
        // Update the list of data points as well as each point
        var points = FindObjectsOfType<DataPoint>();
        foreach (var point in points)
        {
            DataPoints.Add(point);
            point.UpdatePoint(this);
        }
    }

    private void OnEnable()
    {
        #if UNITY_EDITOR
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        #endif
    }

    private void OnDisable()
    {
        #if UNITY_EDITOR
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        #endif
    }
}
