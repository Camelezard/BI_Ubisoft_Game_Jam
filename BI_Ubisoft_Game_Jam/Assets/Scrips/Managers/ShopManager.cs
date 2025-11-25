using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private Transform _houseItemContainer;
    [SerializeField] private Transform _playerItemContainer;
    [SerializeField] private GameObject _houseShopButtonPrefab;
    [SerializeField] private GameObject _playerShopButtonPrefab;
    
    [SerializeField] private List<GameObject> _ShopSection;
    [SerializeField] private GameObject _ActifSection = null;
    
    public static Action OnMonnyChange;
    [SerializeField] private int _StartCurency = 700;
    [SerializeField] private Text _CurencyText;
    private int _Curency = 700;
    private int _CurentBuildingCost = 0;
    
    [SerializeField] private ShopSO _testShopSO;
    
    protected override void Awake()
    {
        base.Awake();
        
        _InitStartCurnecy();
        
        OnMonnyChange += UpdateMonyUi;
        FlowManager.OnShopLoad += LoadShopSO;
        
        OnMonnyChange.Invoke();
        
        LoadShopSO(_testShopSO);
        
        
        gameObject.SetActive(false);
    }
    
    private void LoadShopSO(ShopSO pShopSO)
    {
        int lCount = _houseItemContainer.childCount;
        for (int i = lCount - 1; i >= 0; i--)
        {
            Destroy(_houseItemContainer.GetChild(i).gameObject);
        }
        
        lCount = _playerItemContainer.childCount;
        for (int i = lCount - 1; i >= 0; i--)
        {
            Destroy(_playerItemContainer.GetChild(i).gameObject);
        }
        
        lCount = pShopSO.houseList.Count;
        HouseShopButton lHouseButton;
        for (int i = 0; i < lCount; i++)
        {
            lHouseButton = Instantiate(_houseShopButtonPrefab).GetComponent<HouseShopButton>();
            lHouseButton.transform.SetParent(_houseItemContainer);
            lHouseButton.SetHousePrefab(pShopSO.houseList[i]);
        }
        
        lCount = pShopSO.playerUpgradesList.Count;
        PlayerShopButton lPlayerButton;
        for (int i = 0; i < lCount; i++)
        {
            lPlayerButton = Instantiate(_playerShopButtonPrefab).GetComponent<PlayerShopButton>();
            lPlayerButton.transform.SetParent(_playerItemContainer);
            lPlayerButton.SetPlayerUpgradeSO(pShopSO.playerUpgradesList[i]);
        }
    }

    private void _InitStartCurnecy()
    {
        _Curency = _StartCurency;
    }

    public bool CheckMonny(int pMonnyCost)
    {
        if (pMonnyCost <= _Curency)
        {
            _CurentBuildingCost = pMonnyCost;
            return true;
        }

        return false;
    }

    public bool Buy()
    {
        if (_CurentBuildingCost <= _Curency)
        {
            _Curency -= _CurentBuildingCost;
            OnMonnyChange.Invoke();
            return true;
        }
        return false;
    }
    
    public bool Buy(int pCost)
    {
        if(pCost <= _Curency)
        {
            _Curency -= pCost;
            OnMonnyChange.Invoke();
            return true;
        }
        return false;
    }
    
    private void UpdateMonyUi()
    {
        _CurencyText.text = $"{_Curency} $";
    }

    public void ChangeSection(GameObject pSection )
    {
        _ActifSection.SetActive(false);
        _ActifSection = pSection;
        _ActifSection.SetActive(true);
    }
    
    private void OnDestroy()
    {
        FlowManager.OnShopLoad -= LoadShopSO;
    }
}
