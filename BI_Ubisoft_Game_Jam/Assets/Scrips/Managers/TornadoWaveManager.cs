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
    //public TornadoData tornadoSerializedObject;
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
    [SerializeField] private WaveTimeline _TornadoTimeline;

    [Header("Container")]
    [SerializeField] private GameObject _TornadoContainer;

    [Header("Waves Configuration")]
    public bool loopWaves;

    [Header("Spawn Settings")]
    public Transform targetCenter;
    public float spawnAreaSize = 50f;

    private float lWaveProgress = 0f;
    private List<WaveDataRuntime> wavesRuntime;

    private void Start()
    {
        InitializeRuntimeWaves();
    }

    private void InitializeRuntimeWaves()
    {
        // Crée une copie runtime pour ne jamais toucher aux SO
        wavesRuntime = new List<WaveDataRuntime>();
        foreach (var wave in _TornadoTimeline.waves)
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
            StartCoroutine(LaunchWaveTimeline());
        }
    }

    private IEnumerator LaunchWaveTimeline()
    {
        do
        {
            float elapsedTime = 0f;
            float totalDuration = _TornadoTimeline.timeBeforeNextWave * wavesRuntime.Count; // chaque wave a le même intervalle
            lWaveProgress = 0f;

            int waveIndex = 0;
            float nextWaveTime = _TornadoTimeline.timeBeforeNextWave;

            // Lancer la première vague
            StartCoroutine(LaunchAWave(wavesRuntime[waveIndex]));
            print($"Current wave = {waveIndex + 1}");

            while (elapsedTime < totalDuration)
            {
                elapsedTime += Time.deltaTime;
                lWaveProgress = elapsedTime / totalDuration;

                if (UiManager.Instance != null)
                    UiManager.Instance.UpdateWaveUi(lWaveProgress);

                // Passage à la vague suivante
                if (elapsedTime >= nextWaveTime)
                {
                    waveIndex++;
                    if (waveIndex < wavesRuntime.Count)
                    {
                        StartCoroutine(LaunchAWave(wavesRuntime[waveIndex]));
                        nextWaveTime += _TornadoTimeline.timeBeforeNextWave;
                        print($"Current wave = {waveIndex + 1} : next in {nextWaveTime}s");
                    }
                }

                yield return null;
            }

            print("WaveFinished");

        } while (loopWaves);
    }

    private IEnumerator LaunchAWave(WaveDataRuntime data)
    {
        for (int i = 0; i < data.tornadoPrefabs.Count; i++)
        {
            Tornado prefab = data.tornadoPrefabs[i];
            Tornado tornado = Instantiate(prefab, _TornadoContainer.transform);

            // Spawn aléatoire autour du centre
            if (targetCenter != null)
            {
                Vector3 randomPos = targetCenter.position + new Vector3(
                    Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f),
                    0f,
                    Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f)
                );
                tornado.transform.position = randomPos;
            }

            yield return new WaitForSecondsRealtime(data.spawnInterval);
        }
    }
}

