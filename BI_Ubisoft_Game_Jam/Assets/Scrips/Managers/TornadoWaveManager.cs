using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TornadoWave
{
    public string waveName = "Wave";
    public List<GameObject> tornadoPrefabs; 
    public int count = 5;                    
    public float spawnInterval = 0.5f;       
    public float timeBeforeNextWave = 2f;    
}

public class TornadoWaveManager : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private GameObject _TornadoContainer;

    [Header("Waves Configuration")]
    public List<TornadoWave> waves;
    public bool loopWaves;

    [Header("Spawn Settings")]
    public Transform targetCenter;
    public float spawnAreaSize = 50f;

    private int _CurrentWaveIndex = 0;
    private bool _WaveInProgress = false;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {

        while (true) 
        {
            TornadoWave lWave = waves[_CurrentWaveIndex];

            print($"{lWave.waveName} spawn at {Time.time}");
            _WaveInProgress = true;

            for (int i = 0; i < lWave.count; i++)
            {
                GameObject lPrefab = lWave.tornadoPrefabs[Random.Range(0, lWave.tornadoPrefabs.Count)];

                Vector2 lCircle = Random.insideUnitCircle.normalized * spawnAreaSize;
                Vector3 lSpawnPos = new Vector3(lCircle.x,0,lCircle.y);

                GameObject lTornado = Instantiate(lPrefab, lSpawnPos, Quaternion.identity,_TornadoContainer.transform);

                if (lTornado.TryGetComponent<Tornado>(out Tornado t))
                {
                    Vector3 lDir = (targetCenter.position - lSpawnPos).normalized;
                    t._Direction = new Vector3(lDir.x, 0, lDir.z);
                }

                yield return new WaitForSeconds(lWave.spawnInterval);
            }

            _WaveInProgress = false;

            yield return new WaitForSeconds(lWave.timeBeforeNextWave);

            _CurrentWaveIndex++;
            if (_CurrentWaveIndex >= waves.Count)
                if(loopWaves) _CurrentWaveIndex = 0; 
                else yield break;
        }
    }
}
