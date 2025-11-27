using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class TornadoWave
{
    public string waveName = "Wave";
    public TornadoData tornadoSerializedObject;
    public float waveDuration = 2f;
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
    private float lWaveProgress = 0;

    public static event Action OnWaveEnd;

    //private float _WaveInProgress = false;

    #region singleton

    private static TornadoWaveManager _Instance;
    public static TornadoWaveManager instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no TornadoWaveManager instance found");
                return null;
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("TornadoWaveManager already exists");
        }
    }

    #endregion

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
        foreach (TornadoWave tornadoWave in waves)
        {
            totalDuration += tornadoWave.waveDuration;
        }

        // Set first wave
        TornadoWave currentWave = waves[waveIndex];
        float nextWaveTime = currentWave.waveDuration;

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

                if (waveIndex >= waves.Count)
                    break;

                currentWave = waves[waveIndex];
                nextWaveTime += currentWave.waveDuration;

                //StartCoroutine(LaunchWave(currentWave));

                print($"Current wave = {waveIndex + 1} : tornado to spawn = x : time to wait = {nextWaveTime}");


            }

            yield return null;
        }

        print("WaveFinished");
    }

    public void LaunchWaveEvent(TornadoData pWave)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PhaseSwitch", 1);   //  FMOD
        StartCoroutine(LaunchWaveTimeline(pWave));
    }

    private IEnumerator LaunchWaveTimeline(TornadoData pWave)
    {
        float lElapsedTime = 0f;
        StartCoroutine(LaunchWave(pWave));

        while (lElapsedTime < pWave.waveDuration)
        {
            if (pWave.startTimelineSlider == true)
            {
                lElapsedTime += Time.deltaTime;
                UiManager.Instance.UpdateWaveUi(lElapsedTime / pWave.waveDuration);
            }
            //UiManager.Instance.UpdateWaveUi(lElapsedTime / pWave.waveDuration);
            yield return new WaitForEndOfFrame();
        }

        OnWaveEnd?.Invoke();

        yield return null;
    }

    // private IEnumerator LaunchWave(TornadoData pWave)
    // {
    //     TornadoData data = pWave;
    //     int index = 0;

    //     while (index < data.tornadoPrefabs.Count)
    //     {
    //         // Utilise le spawn interval défini dans ton ScriptableObject !
    //         yield return new WaitForSecondsRealtime(data.spawnInterval);

    //         Tornado prefab = data.tornadoPrefabs[index];
    //         index++;

    //         Tornado tornado = Instantiate(prefab);

    //         // Détermination de la position
    //         Vector2 lCircle2D = UnityEngine.Random.insideUnitCircle.normalized;
    //         Vector3 circle = new Vector3(lCircle2D.x, 0, lCircle2D.y);

    //         Vector3 spawnPos;
    //         Vector3 dir;

    //         if (!data.spawnInWalls)
    //         {
    //             spawnPos = circle * spawnAreaSize;

    //             dir = (targetCenter.position - spawnPos).normalized;
    //             tornado._Direction = new Vector3(dir.x, 0, dir.z);
    //         }
    //         else
    //         {
    //             tornado.canPassAWall = false;

    //             spawnPos = SpawnerManager.Instance.ChooseRandomPositionInSpawners();
    //             //spawnPos = Vector3.zero;

    //             dir = circle;
    //             tornado._Direction = dir;
    //         }

    //         tornado.transform.position = spawnPos;

    //         // Direction
    //         if (tornado.TryGetComponent<Tornado>(out Tornado t))
    //         {

    //         }

    //         yield return null;
    //     }
    // }
    private IEnumerator LaunchWave(TornadoData pWave)
    {
        for (int i = 0; i < pWave.tornadoPrefabs.Count; i++)
        {
            yield return new WaitForSecondsRealtime(pWave.spawnInterval);

            Vector3 spawnPos;
            Vector3 direction;

            if (!pWave.spawnInWalls)
            {
                Vector2 lCircle2D = UnityEngine.Random.insideUnitCircle.normalized;
                Vector3 circle = new Vector3(lCircle2D.x, 0, lCircle2D.y);

                spawnPos = circle * spawnAreaSize;

                direction = (targetCenter.position - spawnPos).normalized;
                direction.y = 0;
            }
            else
            {
                spawnPos = Grid.Instance.GetRandom3DPosInFreeCells();

                Vector2 lCircle2D = UnityEngine.Random.insideUnitCircle.normalized;
                direction = new Vector3(lCircle2D.x, 0, lCircle2D.y);
            }

            Tornado tornado = Instantiate(
                pWave.tornadoPrefabs[i],
                spawnPos,
                Quaternion.identity,
                _TornadoContainer.transform
            );

            tornado.direction = direction;
        }
    }

}
