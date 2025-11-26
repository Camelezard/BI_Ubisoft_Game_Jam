using System;
using UnityEngine;
using UnityEngine.UI;

public class HouseShopButton : MonoBehaviour
{
    [SerializeField] private House _TargetPrefab;
    [SerializeField] protected Image _buttonSprite;
    [SerializeField] private Text priceText;
    [SerializeField] private int _InitialPrice;
    private int _Price;
    
    [HideInInspector] public string itemName, itemDesc;
    
    public static event Action<string, string> OnItemHover;
    public static event Action OnItemHoverEnd;

    void Start()
    {
        // ChangePriceCost(_InitialPrice);
    }

    public virtual void OnButtonPressed()
    {
        if(ShopManager.Instance.CheckMonny(_Price))
        {
            Grid.Instance.ChangSelectHouse(_TargetPrefab);
        }
        //print("pressed");
    }
    
    public void SetHousePrefab(HouseItemSO pHouse)
    {
        _TargetPrefab = pHouse.housePrefab;
        _buttonSprite.sprite = pHouse.icon;
    }
    
    public void ChangePriceCost(int pPrice)
    {
        _Price = pPrice;
        
        priceText.text = $"{_Price} $";
    }
    
    public void OnHover()
    {
        OnItemHover?.Invoke(itemName, itemDesc);
    }
    
    public void OnHoverEnd()
    {
        OnItemHoverEnd?.Invoke();
    }
}

