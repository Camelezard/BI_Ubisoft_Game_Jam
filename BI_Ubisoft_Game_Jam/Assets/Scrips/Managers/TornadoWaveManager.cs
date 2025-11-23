using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoWaveManager : MonoBehaviour
{
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

            waveCoroutine = StartCoroutine(LaunchWaveTimeline());
        }
    }

    private IEnumerator LaunchWaveTimeline()
    {
        float _Elapsed;
        float _WaveFraction;
        float _Progress;

        for (int waveIndex = 0; waveIndex < _tornadoTimeline.waves.Count; waveIndex++)
        {
            WaveData _WaveData = _tornadoTimeline.waves[waveIndex];

            _Elapsed = 0f;
            _WaveFraction = 1f / _tornadoTimeline.waves.Count;

            while (_Elapsed < _WaveData.timeBeforeNextWave + 0.1f)
            {
                _Elapsed += Time.deltaTime;
                _Progress = waveIndex * _WaveFraction + (_Elapsed / _WaveData.timeBeforeNextWave) * _WaveFraction;
                UiManager.Instance?.UpdateWaveUi(_Progress);
                yield return null;
            }

            yield return LaunchAWave(_WaveData);
        }
    }

    private IEnumerator LaunchAWave(WaveData pData)
    {
        for (int i = 0; i < pData.tornadoPrefabs.Count; i++)
        {
            Tornado _Instance = Instantiate(pData.tornadoPrefabs[i], _tornadoContainer.transform);

            if (targetCenter != null)
            {
                Vector2 _RandCircle = Random.insideUnitCircle * spawnAreaSize;
                _Instance.transform.position = targetCenter.position + new Vector3(_RandCircle.x, 0f, _RandCircle.y);
            }

            yield return new WaitForSecondsRealtime(pData.spawnInterval);
        }
    }
}
