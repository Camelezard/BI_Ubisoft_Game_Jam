using UnityEngine;

public enum PlayerUpgrades
{
    Speed,
    RotationSpeed,
    WindZoneLength,    
    WindZoneWidth,
    WindZoneStrength,
    AspirateOnRightClick    
}

[CreateAssetMenu(fileName = "PlayerUpgradeSO", menuName = "Scriptable Objects/PlayerUpgradeSO")]
public class PlayerUpgradeSO : ShopItemSO
{
    public PlayerUpgrades upgradeType;
    [Range(1f, 3f)] public float coeffValue = 1.2f;
}
