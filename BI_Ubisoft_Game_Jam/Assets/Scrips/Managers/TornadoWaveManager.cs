using System;
using System.Collections;
using UnityEngine;

public class TornadoWaveManager : Singleton<TornadoWaveManager>
{
    public static event Action OnWaveEnd;

    [SerializeField] private WaveTimeline _tornadoTimeline;

    [Header("Container")]
    [SerializeField] private GameObject _tornadoContainer;

    [Header("Spawn Settings")]
    public Transform targetCenter;
    public float spawnAreaSize = 50f;

    private Coroutine waveCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (waveCoroutine != null)
                StopCoroutine(waveCoroutine);

            waveCoroutine = StartCoroutine(LaunchWaveTimeline(_tornadoTimeline));
        }
    }

    public IEnumerator LaunchWaveTimeline(WaveTimeline pTmeline)
    {
        float _Elapsed;
        float _WaveFraction;
        float _Progress;

        Debug.Log($"timelin Started with -{pTmeline.name}- prefab");

        for (int waveIndex = 0; waveIndex < pTmeline.waves.Count; waveIndex++)
        {
            WaveData _WaveData = pTmeline.waves[waveIndex];

            _Elapsed = 0f;
            _WaveFraction = 1f / pTmeline.waves.Count;

            while (_Elapsed < _WaveData.timeBeforeNextWave + 0.1f)
            {
                _Elapsed += Time.deltaTime;
                _Progress = waveIndex * _WaveFraction + (_Elapsed / _WaveData.timeBeforeNextWave) * _WaveFraction;
                UiManager.Instance?.UpdateWaveUi(_Progress);
                yield return null;
            }

            yield return LaunchAWave(_WaveData);
        }

        OnWaveEnd.Invoke();
    }

    public IEnumerator LaunchAWave(WaveData pData)
    {
        for (int i = 0; i < pData.tornadoPrefabs.Count; i++)
        {
            Tornado _Instance = Instantiate(pData.tornadoPrefabs[i], _tornadoContainer.transform);

            if (targetCenter != null)
            {
                Vector2 _RandCircle = UnityEngine.Random.insideUnitCircle * spawnAreaSize;
                _Instance.transform.position = targetCenter.position + new Vector3(_RandCircle.x, 0f, _RandCircle.y);
            }

            yield return new WaitForSecondsRealtime(pData.spawnInterval);
        }
    }
}
