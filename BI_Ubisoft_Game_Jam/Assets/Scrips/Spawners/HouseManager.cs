using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>
{
    [SerializeField] private GameObject _HouseContainer;
    [Range(0, 100)]
    [SerializeField] private float _MaxPercentageOfDestruction = 70;

    private float _SmallDestroyPercentage = 0;
    private float _DestroyPercentage = 0;



    private List<House> _InGameHouses = new List<House>();
    private List<House> _DestroyHouse = new List<House>();


    protected override void Awake()
    {
        base.Awake();
        
        if (!_HouseContainer) _HouseContainer = GameObject.Find("HouseContainer");
        _InGameHouses = _HouseContainer.GetComponentsInChildren<House>().ToList();
        _DestroyHouse.Clear();
    }

    public void UpdateDestroyPercentage()
    {
        _SmallDestroyPercentage = (1f - (_InGameHouses.Count - _DestroyHouse.Count) / (float)_InGameHouses.Count) * 100;

        //print(_SmallDestroyPercentage);

        //UiManager.Instance.UpdateDestroyUi(_SmallDestroyPercentage);
        _DestroyPercentage = _SmallDestroyPercentage * 100;

        if (_DestroyPercentage > _MaxPercentageOfDestruction) UiManager.Instance.TriggerDefeat();
    }

    public void AddHouseInList(House pHouse)
    {
        if (!_InGameHouses.Contains(pHouse))
        {
            _InGameHouses.Add(pHouse);
            _DestroyHouse.Remove(pHouse);
        }
        else print("imposible de retirer house");

        UpdateDestroyPercentage();
    }

    public void DestroyHouseInList(House pHouse)
    {

        if (_InGameHouses.Contains(pHouse))
        {
            _InGameHouses.Remove(pHouse);
        }
        else
        { 
            Debug.Log("no houses in _InGameHouses");
        }

    }

    public House RandomHouse()
    { 
        if (_InGameHouses.Count == 0) 
        {
            Debug.LogWarning("no houses in _InGameHouses");
            return null;
        }

        int lRandHouse = UnityEngine.Random.Range(0, _InGameHouses.Count - 1);
        return _InGameHouses[lRandHouse];
    }
}
