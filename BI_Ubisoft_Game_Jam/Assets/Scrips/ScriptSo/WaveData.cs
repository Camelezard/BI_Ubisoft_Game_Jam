using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSO", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public List<Tornado> tornadoPrefabs;
    public float spawnInterval = 0.5f;
    public float timeBeforeNextWave = 2f;
}
