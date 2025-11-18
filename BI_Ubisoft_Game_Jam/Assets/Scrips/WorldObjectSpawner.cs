using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class GaussianCityGenerator : MonoBehaviour
{
    [Header("City Settings")]
    public int houseCount = 100;
    public float cityCenterSize = 10f; 
    public float minDistanceBetweenHouses = 2f;

    [Header("Prefabs")]
    public GameObject[] housePrefabs;

    private List<Vector3> placedHousesPos = new List<Vector3>();

    void Start()
    {
        GenerateCity();
    }

    void GenerateCity()
    {
        int tries = 0;

        for (int i = 0; i < houseCount; i++)
        {
            bool placed = false;

            while (!placed)
            {
                tries++;
                if (tries > houseCount * 10)
                {
                    Debug.LogWarning("Trop de tentatives, stop placement.");
                    return;
                }

                Vector3 pos = GenerateGaussianPosition();

                if (IsValidPosition(pos))
                {
                    GameObject prefab = housePrefabs[Random.Range(0, housePrefabs.Length)];
                    Instantiate(prefab, pos, Quaternion.identity);

                    placedHousesPos.Add(pos);
                    placed = true;
                }
            }
        }
    }

    Vector3 GenerateGaussianPosition()
    {
        float x = GaussianRandom(0f, cityCenterSize);
        float z = GaussianRandom(0f, cityCenterSize);

        return new Vector3(x, 0f, z);
    }

    float GaussianRandom(float mean, float stdDev)
    {
        float u1 = Random.value;
        float u2 = Random.value;

        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        return mean + stdDev * randStdNormal;
    }

    bool IsValidPosition(Vector3 pos)
    {
        foreach (var p in placedHousesPos)
        {
            if (Vector3.Distance(p, pos) < minDistanceBetweenHouses)
                return false;
        }
        return true;
    }
}
