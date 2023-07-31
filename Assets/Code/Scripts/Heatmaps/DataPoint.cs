using UnityEngine;

public class DataPoint : MonoBehaviour
{
    public void UpdatePoint(DataPointCollection pointCollection)
    {
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
