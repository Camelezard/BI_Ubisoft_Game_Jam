using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>
{
    [SerializeField] private GameObject _HouseContainer;
    [SerializeField] private GameObject _IconHousePrefab;
    // [SerializeField] private GameObject _IconDestroyHousePrefab;
    [SerializeField] private LayoutGroupAutoReduction _IconContainer;
    [Range(0, 100)]
    [SerializeField] private float _MaxPercentageOfDestruction = 70;

    private float _SmallDestroyPercentage = 0;
    private float _DestroyPercentage = 0;



    private List<House> _InGameHouses = new List<House>();
    private List<House> _DestroyHouse = new List<House>();
    private List<GameObject> _HouseIconImage = new List<GameObject>();


    protected override void Awake()
    {
        base.Awake();

        if (!_HouseContainer) _HouseContainer = GameObject.Find("HouseContainer");
        _InGameHouses = _HouseContainer.GetComponentsInChildren<House>().ToList();
        _DestroyHouse.Clear();
    }

    public void UpdateDestroyPercentage()
    {
        int totalCurrent = _InGameHouses.Count + _DestroyHouse.Count;

        if (totalCurrent == 0)
            totalCurrent = 1;

        _DestroyPercentage = ((float)_DestroyHouse.Count / (float)totalCurrent) * 100f;

        if (_DestroyPercentage > _MaxPercentageOfDestruction)
        {
            UiManager.Instance.TriggerDefeat();
        }
    }

    public void AddHouseInList(House pHouse)
    {
        if (!_InGameHouses.Contains(pHouse))
        {
            _InGameHouses.Add(pHouse);
            //_DestroyHouse.Remove(pHouse);


        }
        else print("imposible de retirer house");

        UpdateDestroyPercentage();
    }

    public void DestroyHouseInList(House pHouse)
    {

        if (_InGameHouses.Contains(pHouse))
        {
            GameObject lHomeIcone = _HouseIconImage[0];

            //lHomeIcone = Instantiate(_IconDestroyHousePrefab, Vector3.zero, Quaternion.identity, _IconContainer.gameObject.transform);
            _HouseIconImage.Add(lHomeIcone);

            _DestroyHouse.Add(pHouse);
            _InGameHouses.Remove(pHouse);
        }
        else
        {
            //Debug.Log("no houses in _InGameHouses");
        }
        UpdateDestroyPercentage();
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

    private void UpdateLifeContainer(int pHp)
    {
        if(_IconContainer.GetComponentInChildren<GameObject>.ToList() < pHp)



        _HouseIconImage.Add(lHomeIcone);
    }
}
