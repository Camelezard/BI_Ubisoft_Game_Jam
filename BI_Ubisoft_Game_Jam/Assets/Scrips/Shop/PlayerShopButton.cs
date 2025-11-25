using UnityEngine;
using UnityEngine.UI;

public class PlayerShopButton : HouseShopButton
{
    private PlayerUpgradeSO _playerUpgradeSO;
    
    public override void OnButtonPressed()
    {
        if(!ShopManager.Instance.Buy(_playerUpgradeSO.price)) return;
        PlayerUpgradeManager.ApplyUpgrade(_playerUpgradeSO);
        _buttonSprite.GetComponent<Button>().interactable = false;
    }
    
    public void SetPlayerUpgradeSO(PlayerUpgradeSO pUpgrade)
    {
        _playerUpgradeSO = pUpgrade;
        _buttonSprite.sprite = pUpgrade.icon;
    }
}
