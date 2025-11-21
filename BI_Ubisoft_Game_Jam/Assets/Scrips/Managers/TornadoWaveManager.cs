using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    public List<Tornado> tornadoPrefabs;
    public float spawnInterval = 0.5f;
    public float timeBeforeNextWave = 2f;

}

[CreateAssetMenu(fileName = "WaveTimeline")]
public class WaveTimeline : ScriptableObject
{
    public string waveName = "Wave";
    public List<WaveData> waves;
}

[System.Serializable]
public class WaveDataRuntime
{
    public List<Tornado> tornadoPrefabs;
    public float spawnInterval;
    public float timeBeforeNextWave = 2f;

}

public class TornadoWaveManager : MonoBehaviour
{
    [SerializeField] private WaveTimeline _tornadoTimeline;

    [Header("Container")]
    [SerializeField] private GameObject _tornadoContainer;

    [Header("Waves Configuration")]

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
                timeBeforeNextWave = wave.timeBeforeNextWave,
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
        WaveDataRuntime _WaveData;

        float _Elapsed;
        float _WaveFraction;
        float _Progress;

        for (int waveIndex = 0; waveIndex < wavesRuntime.Count; waveIndex++)
        {
            _WaveData = wavesRuntime[waveIndex];


            _Elapsed = 0f;
            _WaveFraction = 1f / wavesRuntime.Count;

            while (_Elapsed < _WaveData.timeBeforeNextWave+ .1f)
            {
                _Elapsed += Time.deltaTime;
                _Progress = waveIndex * _WaveFraction + (_Elapsed / _WaveData.timeBeforeNextWave) * _WaveFraction;
                UiManager.Instance?.UpdateWaveUi(_Progress);
                yield return null;
            }
            
            yield return LaunchAWave(_WaveData);
        }
    }

    private IEnumerator LaunchAWave(WaveDataRuntime pData)
    {
        Tornado _Instance;
        Vector2 _RandCircle;

        for (int i = 0; i < pData.tornadoPrefabs.Count; i++)
        {
            _Instance = Instantiate(pData.tornadoPrefabs[i], _tornadoContainer.transform);

            if (targetCenter != null)
            {
                _RandCircle = Random.insideUnitCircle * spawnAreaSize;
                _Instance.transform.position = targetCenter.position + new Vector3(_RandCircle.x, 0f, _RandCircle.y);
            }

            yield return new WaitForSecondsRealtime(pData.spawnInterval);
        }
    }
}
