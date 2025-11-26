using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class HouseManager : Singleton<HouseManager>
{
    [SerializeField] private GameObject _HouseContainer;
    [SerializeField] private GameObject _IconHousePrefab;
    [SerializeField] private GameObject _IconDestroyHousePrefab;
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

        AdjustHouses(GetRemainingHousesBeforeDefeat());

        if (_DestroyPercentage > _MaxPercentageOfDestruction)
        {
            UiManager.Instance.TriggerDefeat();
        }
    }

    public int GetRemainingHousesBeforeDefeat()
    {
        int totalCurrent = _InGameHouses.Count + _DestroyHouse.Count;

        if (totalCurrent == 0)
            return 0;

        float maxDestroyedAllowed = (totalCurrent * _MaxPercentageOfDestruction) / 100f;

        int maxDestroyedInt = Mathf.FloorToInt(maxDestroyedAllowed);

        int remaining = maxDestroyedInt - _DestroyHouse.Count + 1;

        return Mathf.Max(remaining, 0);
    }

    public void AddHouseInList(House pHouse)
    {
        if (!_InGameHouses.Contains(pHouse))
        {
            _InGameHouses.Add(pHouse);
            
        }
        else print("imposible de retirer house");

        UpdateDestroyPercentage();
    }

    public void DestroyHouseInList(House pHouse)
    {

        if (_InGameHouses.Contains(pHouse))
        {



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

    public void AdjustHouses(int pNumber)
    {
        int currentCount = _HouseIconImage.Count;

        if (currentCount > pNumber)
        {
            int toRemove = currentCount - pNumber;

            for (int i = 0; i < toRemove; i++)
            {
                GameObject go = _HouseIconImage[_HouseIconImage.Count - 1];
                _HouseIconImage.RemoveAt(_HouseIconImage.Count - 1);
                Destroy(go);
            }
        }

        else if (currentCount < pNumber)
        {
            int toAdd = pNumber - currentCount;

            for (int i = 0; i < toAdd; i++)
            {
                GameObject go = Instantiate(
                    _IconHousePrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    _IconContainer.transform
                );

                _HouseIconImage.Add(go);
            }
        }
    }

    public void RepareHouse(House pPreviusHouse)
    {
        if (_DestroyHouse.Contains(pPreviusHouse))
        {
            _DestroyHouse.Remove(pPreviusHouse);
        }
        else Debug.Log("no house to reper");
    }
}
