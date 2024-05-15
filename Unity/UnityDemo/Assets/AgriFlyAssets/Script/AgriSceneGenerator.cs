﻿using UnityEngine;

public class AgriSceneGenerator : MonoBehaviour
{
    // 1. Tree Prefabs
    public GameObject almondPrefab;
    public GameObject applePrefab;
    public GameObject lemonPrefab;
    public GameObject olivePrefab;
    public GameObject orangePrefab;
    public GameObject peachPrefab;
    public GameObject strawberryPrefab;

    // An enum to represent the type of tree
    public enum TreeType { Almond, Apple, Lemon, Olive, Orange, Peach, Strawberry }
    public TreeType selectedTreeType;

    // 2. Matrix size
    [Header("Matrix Settings")]
    public int numAlongX = 5; // Number of trees along world x-axis
    public int numAlongZ = 5;  // Number of trees along world z-axis

    // 3. Distance between trees
    public float distanceAlongX = 10f; // Distance between trees along x-axis
    public float distanceAlongZ = 10f; // Distance between trees along z-axis

    // 4. Position of the matrix center
    [Header("Center Position")]
    public Vector3 matrixCenterPosition = new Vector3(300.0f,0.0f,60.0f);

    // 5. Size distribution settings
    [Header("Size Distribution Settings")]
    public float meanSize = 1f;
    public float varianceSize = 0.2f;  // Standard deviation


    private void Start()
    {
        PopulateMatrix();
    }

    public void PopulateMatrix()
    {
        for (int i = 0; i < numAlongX; i++)
        {
            for (int j = 0; j < numAlongZ; j++)
            {
                // Calculate position
                Vector3 position = new Vector3(matrixCenterPosition.x + (i - numAlongX / 2) * distanceAlongX,
                                               matrixCenterPosition.y,
                                               matrixCenterPosition.z + (j - numAlongZ / 2) * distanceAlongZ);

                // Instantiate the selected tree type with the given orientation
                Quaternion initialOrientation = GetSelectedTreePrefab().transform.rotation;
                GameObject treeInstance = Instantiate(GetSelectedTreePrefab(), position, initialOrientation, transform);
                treeInstance.name = selectedTreeType.ToString() + "_" + i + "_" + j;

                // Adjust tree size based on the normal distribution
                float randomSizeFactor = Mathf.Clamp(NormalDistributionRandom(meanSize, varianceSize), 0.5f, 2f); // just to ensure size remains reasonable
                Vector3 originalScale = treeInstance.transform.localScale; // Get the prefab's original scale
                treeInstance.transform.localScale = new Vector3(originalScale.x * randomSizeFactor, originalScale.y * randomSizeFactor, originalScale.z * randomSizeFactor);

            }
        }
    }


    private GameObject GetSelectedTreePrefab()
    {
        switch (selectedTreeType)
        {
            case TreeType.Almond: return almondPrefab;
            case TreeType.Apple: return applePrefab;
            case TreeType.Lemon: return lemonPrefab;
            case TreeType.Olive: return olivePrefab;
            case TreeType.Orange: return orangePrefab;
            case TreeType.Peach: return peachPrefab;
            case TreeType.Strawberry: return strawberryPrefab;
            default: return null;
        }
    }

    // Function to get a random number from a normal distribution
    private float NormalDistributionRandom(float mean, float standardDeviation)
    {
        float u1 = 1.0f - Random.value; //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        return mean + standardDeviation * randStdNormal; //random normal(mean,stdDev^2)
    }

}