using System;
using UnityEngine;

[ExecuteInEditMode]
public class DataPoint : MonoBehaviour
{
    private DataPointCollection _pointCollection;

    private void Awake()
    {
        _pointCollection = FindObjectOfType<DataPointCollection>();
    }
    
    private void OnDrawGizmos()
    {
        if (!_pointCollection.VisualizeTargetRadius) return;
        
        Gizmos.DrawWireSphere(transform.position, _pointCollection.PointTargetRadius);
    }

    public void UpdatePoint(DataPointCollection pointCollection)
    {
        _pointCollection = pointCollection;
        
        // Check how many points are nearby and adjust the size and color accordingly
        var pointConcentration = 0f;
        foreach (var point in pointCollection.DataPoints)
        {
            if (Vector3.Distance(point.transform.position, transform.position) < pointCollection.PointTargetRadius)
            {
                pointConcentration++;
            }
        }

        var concentrationSpectrum =
            Mathf.Clamp(pointConcentration / pointCollection.MaxPointConcentrationSpectrum, 0f, 1f);
        
        // Adjust the color of the point marker
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.Lerp(pointCollection.SpectrumBorder, pointCollection.SpectrumCenter,
            concentrationSpectrum);
        
        // Slightly increase the scale of the points with higher concentration
        transform.localScale = pointCollection.PointBaseScale +
                               Vector3.Lerp(Vector3.zero, pointCollection.MaxScaleSpectrum, concentrationSpectrum);
    }
}
