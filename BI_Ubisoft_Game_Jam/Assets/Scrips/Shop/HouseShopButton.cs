using UnityEngine;
using UnityEngine.UI;

public class HouseShopButton : MonoBehaviour
{
    [SerializeField] private House _TargetPrefab;
    [SerializeField] private Image _buttonSprite;
    [SerializeField] private Text priceText;
    [SerializeField] private int _InitialPrice;
    private int _Price;

    void Start()
    {
        ChangePriceCost(_InitialPrice);
    }

    public void OnButtonPressed()
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
        _buttonSprite.sprite = pHouse.houseIcon;
    }

    public void ChangePriceCost(int pPrice)
    {
        _Price = pPrice;

        priceText.text = $"{_Price} $";
    }
}

