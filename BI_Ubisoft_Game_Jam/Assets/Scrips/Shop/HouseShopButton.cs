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

    public static event Action<string, string, int> OnItemHover;
    public static event Action OnItemHoverEnd;

    // void Start()
    // {
    //     ChangePriceCost(_InitialPrice);
    // }

    public virtual void OnButtonPressed()
    {
        // Vérifications de sécurité
        if (Grid.Instance == null)
        {
            Debug.LogError("Grid.Instance is null! Assurez-vous que le Grid est actif dans la scène.");
            return;
        }

        if (_TargetPrefab == null)
        {
            Debug.LogError("TargetPrefab n'est pas assigné pour ce bouton!");
            return;
        }

        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance is null!");
            return;
        }

        // Vérifier que le joueur a assez d'argent
        if (ShopManager.Instance.CheckMonny(_Price))
        {
            Grid.Instance.ChangSelectHouse(_TargetPrefab);
        }
    }

    public void SetHousePrefab(HouseItemSO pHouse)
    {
        if (pHouse == null)
        {
            Debug.LogError("HouseItemSO est null!");
            return;
        }

        _TargetPrefab = pHouse.housePrefab;
        _buttonSprite.sprite = pHouse.icon;
        itemName = pHouse.itemName;
        //itemDesc = pHouse.description;
    }

    public void ChangePriceCost(int pPrice)
    {
        _Price = pPrice;
        if (priceText != null)
            priceText.text = $"{_Price}";
    }

    public void OnHover()
    {
        OnItemHover?.Invoke(itemName, itemDesc, _Price);
    }

    public void OnHoverEnd()
    {
        OnItemHoverEnd?.Invoke();
    }
}
