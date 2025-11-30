using UnityEngine;

public static class PlayerUpgradeManager
{
    public static void ApplyUpgrade(PlayerUpgradeSO pUpgrade)
    {
        switch (pUpgrade.upgradeType)
        {
            case PlayerUpgrades.Speed :
                PerimeterFollower.instance.SetSpeed(pUpgrade.coeffValue);
                break;
            case PlayerUpgrades.RotationSpeed :
                PlayerCanon.Instance.SetRotationSpeed(pUpgrade.coeffValue);
                break;
            case PlayerUpgrades.WindZoneLength :
                PlayerWindZone.instance.SetLength(pUpgrade.coeffValue);
                break;
            case PlayerUpgrades.WindZoneWidth :
                PlayerWindZone.instance.SetWidth(pUpgrade.coeffValue);
                break;
            case PlayerUpgrades.WindZoneStrength :
                PlayerWindZone.instance.SetStrength(pUpgrade.coeffValue);
                break;
            case PlayerUpgrades.AspirateOnRightClick :
                PlayerWindZone.instance._canAspirate = true;
                break;
            default:
                break;
        }
    }
}
