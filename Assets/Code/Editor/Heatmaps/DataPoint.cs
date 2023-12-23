using UnityEngine;

//*******************************************************************************************
// DataPoint
//*******************************************************************************************
namespace KrillOrBeKrilled.Editor.Heatmaps {
    /// <summary>
    /// Represents a data point in a heatmap, adjusting the scale and color of the
    /// associated GameObject with respect to the concentration of other
    /// <see cref="DataPoint">DataPoints</see> in a specified range.
    /// </summary>
    [ExecuteInEditMode]
    public class DataPoint : MonoBehaviour {
        private DataPointCollection _pointCollection;

        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            this._pointCollection = FindObjectOfType<DataPointCollection>();
        }

        private void OnDrawGizmos() {
            if (!this._pointCollection.VisualizeTargetRadius) {
                return;
            }

            Gizmos.DrawWireSphere(this.transform.position, this._pointCollection.PointTargetRadius);
        }
        
        #endregion
        
        //========================================
        // Internal Methods
        //========================================
        
        #region Internal Methods
        
        /// <summary>
        /// Calculates the concentration of points around this <see cref="DataPoint"/> and interpolates
        /// the color and scale between maximum and minimum values specified in the provided
        /// <see cref="DataPointCollection"/>.
        /// </summary>
        /// <param name="pointCollection"> The <see cref="DataPointCollection"/> containing this
        /// <see cref="DataPoint"/> and its color, scale, and concentration configurations. </param>
        /// <remarks>
        /// Invoked by <see cref="DataPointCollection"/> every time the scene is updated in the Unity Editor.
        /// </remarks>
        internal void UpdatePoint(DataPointCollection pointCollection) {
            this._pointCollection = pointCollection;

            // Check how many points are nearby and adjust the size and color accordingly
            var pointConcentration = 0f;
            foreach (var point in pointCollection.DataPoints) {
                if (Vector3.Distance(point.transform.position, this.transform.position) < pointCollection.PointTargetRadius) {
                    pointConcentration++;
                }
            }

            var concentrationSpectrum =
                Mathf.Clamp(pointConcentration / pointCollection.MaxPointConcentrationSpectrum, 0f, 1f);

            // Adjust the color of the point marker
            var sprite = this.GetComponent<SpriteRenderer>();
            sprite.color = Color.Lerp(pointCollection.SpectrumBorder, pointCollection.SpectrumCenter,
                concentrationSpectrum);

            // Slightly increase the scale of the points with higher concentration
            this.transform.localScale = pointCollection.PointBaseScale +
                Vector3.Lerp(Vector3.zero, pointCollection.MaxScaleSpectrum, concentrationSpectrum);
        }
        
        #endregion
    }
}
