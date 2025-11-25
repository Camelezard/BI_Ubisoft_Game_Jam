using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopSO", menuName = "Scriptable Objects/ShopSO")]
public class ShopSO : ScriptableObject
{
    public List<HouseItemSO> houseList;
    public List<PlayerUpgradeSO> playerUpgradesList;
}
