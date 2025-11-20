using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    public List<Tornado> tornadoPrefabs;
    public bool spawnInWalls = false;
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

    private float lWaveProgress = 0;

    //private float _WaveInProgress = false;

    void Start()
    {
        //StartCoroutine(WaveRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) StartCoroutine(LaunchWaveTimeline());
    }

    private IEnumerator LaunchWaveTimeline()
    {
        //_WaveInProgress = true;

        float elapsedTime = 0f;
        float totalDuration = 0f;
        lWaveProgress = 0;

        int waveIndex = 0;

        // Calculate total duration
        foreach (WaveData tornadoWave in _TornadoTimeline.waves)
        {
            totalDuration += _TornadoTimeline.timeBeforeNextWave;
        }

        // Set first wave
        WaveData currentWave = _TornadoTimeline.waves[waveIndex];
        float nextWaveTime = _TornadoTimeline.timeBeforeNextWave;

        print($"Current wave = {waveIndex + 1}");

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            lWaveProgress = elapsedTime / totalDuration;

            UiManager.Instance.UpdateWaveUi(lWaveProgress);

            // Check if it's time for the next wave
            if (elapsedTime >= nextWaveTime)
            {
                waveIndex++;

                if (waveIndex >= _TornadoTimeline.waves.Count)
                    break;

                currentWave = _TornadoTimeline.waves[waveIndex];
                nextWaveTime += _TornadoTimeline.timeBeforeNextWave;

                StartCoroutine(LunchAWave(currentWave));

                print($"Current wave = {waveIndex + 1} : tornado to spawn = x : time to wait = {nextWaveTime}");
            }

            yield return null;
        }

        print("WaveFinished");
    }

    private IEnumerator LunchAWave(WaveData data)
    {
        int index = 0;

        while (index < data.tornadoPrefabs.Count)
        {
            // Utilise le spawn interval défini dans ton ScriptableObject !
            yield return new WaitForSecondsRealtime(data.spawnInterval);

            Tornado prefab = data.tornadoPrefabs[index];
            index++;

            Tornado tornado = Instantiate(prefab);

            // Détermination de la position
            Vector2 lCircle2D = Random.insideUnitCircle.normalized;
            Vector3 circle = new Vector3(lCircle2D.x, 0, lCircle2D.y);

            Vector3 spawnPos;
            Vector3 dir;

            if (!data.spawnInWalls)
            {
                spawnPos = circle * spawnAreaSize;

                dir = (targetCenter.position - spawnPos).normalized;
                tornado._Direction = new Vector3(dir.x, 0, dir.z);
            }
            else
            {
                tornado.canPassAWall = false;

                spawnPos = SpawnerManager.Instance.ChoseRandomPositinInSpawnwers();
                //spawnPos = Vector3.zero;

                dir = circle;
                tornado._Direction = dir;
            }

            tornado.transform.position = spawnPos;

            // Direction
            if (tornado.TryGetComponent<Tornado>(out Tornado t))
            {

            }

            yield return null;
        }
    }

}
