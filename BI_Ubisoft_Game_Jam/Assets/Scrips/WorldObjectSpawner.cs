using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class WorldObjectSpawner : MonoBehaviour
{
    [Header("ObjectToSpaw")]
    [SerializeField] private House _HouseFactory;
    [SerializeField] private int _HouseNumber = 10;

    [SerializeField] private Tornado _TornadoToSpawn;
    [SerializeField] private int _TornadoNumber = 3;


    [Header("SpawnRange")]
    [SerializeField] private float _SqareRange = 3;



    void Start()
    {
        //InitWorld();
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
