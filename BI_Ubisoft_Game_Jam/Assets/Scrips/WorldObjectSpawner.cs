using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class WorldObjectSpawner : MonoBehaviour
{
    [Header("ObjectToSpaw")]
    private House _HouseFactory;
    private int _HouseNumber = 10;

    private Tornado _TornadoToSpawn;
    private int _TornadoNumber = 3;


    [Header("SpawnRange")]
    private float _SqareRange = 3;



    void Start()
    {
        InitWorld();
    }

    private void InitWorld()
    {
        SpawnStartTornados();
        SpawnStartHouses();
    }

    private void SpawnStartTornados()
    {
        Tornado lTornado; 
        for (int i = 0; i < _TornadoNumber; i++)
        {
            lTornado = Instantiate(_TornadoToSpawn);
        }
    }
    private void SpawnStartHouses()
    {
        House lHouse; 
        for (int i = 0; i < _TornadoNumber; i++)
        {
            lHouse = Instantiate(_HouseFactory);
        }
    }
}
