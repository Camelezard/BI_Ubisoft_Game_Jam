using UnityEngine;
using System.Collections.Generic;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 20;       // Largeur du plan
    public int height = 20;      // Hauteur du plan
    public float spacing = 2f;   // Espacement entre les bâtiments

    [Header("Noise Settings")]
    public float scale = 0.2f;   // Échelle du Perlin Noise

    [Header("Prefab Lists")]
    public List<GameObject> grandPrefabs;   // Liste des grands bâtiments
    public List<GameObject> moyenPrefabs;   // Liste des moyens
    public List<GameObject> petitPrefabs;   // Liste des petits

    [Header("Height Thresholds")]
    public float petitThreshold = 0.3f;
    public float moyenThreshold = 0.6f;

    void Start()
    {
        SpawnBuildings();
    }

    void SpawnBuildings()
    {
        float offsetX = (width - 1) * spacing / 2f;
        float offsetZ = (height - 1) * spacing / 2f;

        float minNoiseMultiplier = 0.2f; 
        float falloff = 2f; 

        Vector2 center = new Vector2(offsetX, offsetZ); 

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale, z * scale);

                float dx = x * spacing - center.x;
                float dz = z * spacing - center.y;
                float distance = Mathf.Sqrt(dx * dx + dz * dz);

                float maxDistance = Mathf.Sqrt(offsetX * offsetX + offsetZ * offsetZ); 
                float multiplier = Mathf.Lerp(1f, minNoiseMultiplier, Mathf.Pow(distance / maxDistance, falloff));

                noiseValue *= multiplier;

                GameObject prefabToSpawn = null;
                if (noiseValue < petitThreshold && petitPrefabs.Count > 0)
                {
                    prefabToSpawn = petitPrefabs[Random.Range(0, petitPrefabs.Count)];
                }
                else if (noiseValue < moyenThreshold && moyenPrefabs.Count > 0)
                {
                    prefabToSpawn = moyenPrefabs[Random.Range(0, moyenPrefabs.Count)];
                }
                else if (grandPrefabs.Count > 0)
                {
                    prefabToSpawn = grandPrefabs[Random.Range(0, grandPrefabs.Count)];
                }

                if (prefabToSpawn != null)
                {
                    Vector3 spawnPosition = new Vector3(x * spacing - offsetX, 0, z * spacing - offsetZ);
                    GameObject lHouse = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                    lHouse.transform.SetParent(Grid.Instance._HouseCOntainer.transform);
                }
            }
        }
    }

}
