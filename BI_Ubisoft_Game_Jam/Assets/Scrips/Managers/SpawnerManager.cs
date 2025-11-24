using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : Singleton<SpawnerManager>
{
    private List<TornadoSpawner> _TornadoSpasnerList;

    [SerializeField] private GameObject _TornadoSpawnerContainer;
    void Start()
    {
        InitTornadoSpasnerList();
    }

    private void InitTornadoSpasnerList()
    {
        _TornadoSpasnerList = new List<TornadoSpawner>(_TornadoSpawnerContainer.GetComponentsInChildren<TornadoSpawner>());
    }

    public Vector3 ChoseRandomPositinInSpawnwers()
    {
        if(_TornadoSpasnerList.Count == 0) 
        {
            Debug.Log ($"pas de tormnadospawner detecter");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, _TornadoSpasnerList.Count - 1);

        TornadoSpawner lRandTornadoSpawner = _TornadoSpasnerList[randomIndex];
        return lRandTornadoSpawner.GetRandomPosInRange();
    }
}
