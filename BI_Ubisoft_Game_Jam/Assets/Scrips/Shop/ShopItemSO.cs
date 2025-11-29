using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public int price = 100;
    public Sprite icon;
    public string itemName;
    public string itemDescription;
}
