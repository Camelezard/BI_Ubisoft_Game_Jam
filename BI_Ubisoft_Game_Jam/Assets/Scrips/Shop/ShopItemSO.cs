using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public int price;
    public Sprite icon;
    public string itemName;
    [TextArea] public string itemDescription;
}
