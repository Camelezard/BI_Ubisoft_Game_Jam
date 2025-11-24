using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TornadoData", menuName = "Scriptable Objects/TornadoData")]
public class TornadoData : ScriptableObject
{
    public List<Tornado> tornadoPrefabs;
    public bool spawnInWalls = false;
    public float spawnInterval = 0.5f;
    public float waveDuration = 2f;
}