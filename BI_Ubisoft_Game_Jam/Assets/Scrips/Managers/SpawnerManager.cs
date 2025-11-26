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
        print(_TornadoSpasnerList.Count);
    }

//     public Vector3 ChooseRandomPositionInSpawners()
//     {
//         if (_TornadoSpasnerList.Count == 0)
//         {
//             Debug.LogWarning("Pas de TornadoSpawner détecté !");
//             return Vector3.zero;
//         }

//         int randomIndex = Random.Range(0, _TornadoSpasnerList.Count);
//         TornadoSpawner randomSpawner = _TornadoSpasnerList[randomIndex];

//         return randomSpawner.GetRandomPosInRange();
//     }
 }
