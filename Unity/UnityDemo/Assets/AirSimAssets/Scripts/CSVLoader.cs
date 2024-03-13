using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.TubeRenderer;

[RequireComponent(typeof(TubeRenderer))] 
public class CSVLoader : MonoBehaviour
{
    public string fileName = "test_traj.csv";

    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        LoadCSV(filePath);
    }

    void LoadCSV(string filePath)
    {
        List<Vector3> positions = new List<Vector3>();
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                if (values.Length >= 3)
                {
                    float x, y, z;
                    if (float.TryParse(values[0], out x) &&
                        float.TryParse(values[1], out y) &&
                        float.TryParse(values[2], out z))
                    {
                        positions.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        // Use the positions here, for example, assigning them to your TubeRenderer
        // Assuming you have a reference to your TubeRenderer script
        TubeRenderer tubeRenderer = GetComponent<TubeRenderer>();
        if (tubeRenderer != null)
        {
            tubeRenderer.SetPositions(positions.ToArray());
        }
    }
}