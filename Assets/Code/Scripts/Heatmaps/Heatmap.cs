using System.Collections.Generic;
using UnityEngine;
namespace Heatmaps {
    public class Heatmap : MonoBehaviour
    {
        public Object CSVFile;
        public List<Object> FilesParsed;

        [SerializeField] private string _filePath;

        public DataPointCollection PointCollection;
        public GameObject DataPoint;

        public void GenerateHeatmap()
        {
            var lines = System.IO.File.ReadAllLines(this._filePath);

            for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                if (lineNumber < 1) continue;

                var elements = lines[lineNumber].Split(',');
                float.TryParse(elements[0], out var x);
                float.TryParse(elements[1], out var y);
                float.TryParse(elements[2], out var z);

                // Create a data point for each position on the map
                var point = Instantiate(this.DataPoint, new Vector3(x, y, z), Quaternion.identity);

                // Set the point's data and add the new point to a list of points
                point.transform.SetParent(this.PointCollection.transform);
            }

            this.FilesParsed.Add(this.CSVFile);
        }

        public void ClearHeatmap()
        {
            this.PointCollection.ClearPoints();
            this.FilesParsed.Clear();
        }
    }
}
