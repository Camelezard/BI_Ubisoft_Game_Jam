using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    public List<Tornado> tornadoPrefabs; 
    public float spawnInterval = 0.5f;
}

[CreateAssetMenu(fileName = "WaveTimeline")]
public class WaveTimeline : ScriptableObject
{
    public string waveName = "Wave";
    public float timeBeforeNextWave = 2f;
    public List<WaveData> waves; 
}

[System.Serializable]
public class WaveDataRuntime
{
    public List<Tornado> tornadoPrefabs; 
    public float spawnInterval;
}

public class TornadoWaveManager : MonoBehaviour
{
    [SerializeField] private WaveTimeline _tornadoTimeline;

    [Header("Container")]
    [SerializeField] private GameObject _tornadoContainer;

    [Header("Waves Configuration")]
    public bool loopWaves;

    [Header("Spawn Settings")]
    public Transform targetCenter;
    public float spawnAreaSize = 50f;

    private List<WaveDataRuntime> wavesRuntime;
    private Coroutine waveCoroutine;

    private void Start()
    {
        InitializeRuntimeWaves();
    }

    private void InitializeRuntimeWaves()
    {
        wavesRuntime = new List<WaveDataRuntime>();
        foreach (var wave in _tornadoTimeline.waves)
        { 
            wavesRuntime.Add(new WaveDataRuntime
            {
                tornadoPrefabs = new List<Tornado>(wave.tornadoPrefabs),
                spawnInterval = wave.spawnInterval
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (waveCoroutine != null)
                StopCoroutine(waveCoroutine);

            waveCoroutine = StartCoroutine(LaunchWaveTimeline());
        }
    }

    private IEnumerator LaunchWaveTimeline()
    {
        do
        {
            for (int waveIndex = 0; waveIndex < wavesRuntime.Count; waveIndex++)
            {
                WaveDataRuntime waveData = wavesRuntime[waveIndex];
                StartCoroutine(LaunchAWave(waveData));

                float elapsed = 0f;
                while (elapsed < _tornadoTimeline.timeBeforeNextWave)
                {
                    elapsed += Time.deltaTime;

                    float progress = (float)waveIndex / wavesRuntime.Count + (elapsed / _tornadoTimeline.timeBeforeNextWave) / wavesRuntime.Count;
                    if (UiManager.Instance != null)
                        UiManager.Instance.UpdateWaveUi(progress);

                    yield return null;
                }
            }
        } while (loopWaves);
    }

    private IEnumerator LaunchAWave(WaveDataRuntime data)
    {
        for (int i = 0; i < data.tornadoPrefabs.Count; i++)
        {
            Tornado prefab = data.tornadoPrefabs[i];
            Tornado tornadoInstance = Instantiate(prefab, _tornadoContainer.transform);

            if (targetCenter != null)
            {
                Vector3 randomPos = targetCenter.position + new Vector3(
                    Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f),
                    0f,
                    Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f)
                );
                tornadoInstance.transform.position = randomPos;
            }

            yield return new WaitForSecondsRealtime(data.spawnInterval);
        }
    }
}
