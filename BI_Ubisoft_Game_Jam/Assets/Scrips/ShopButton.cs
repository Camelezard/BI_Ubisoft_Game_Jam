using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private House _TargetPrefab;
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
        
    }

    public void ChangePriceCost(int pPrice)
    {
        _Price = pPrice;

        priceText.text = $"{_Price} $";
    }
}

