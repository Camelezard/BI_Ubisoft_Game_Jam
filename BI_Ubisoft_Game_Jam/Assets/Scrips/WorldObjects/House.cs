using UnityEngine;

public class House : WorldObject
{
    [Header("Gold")] public int goldGainOnWaveEnd;
    
    protected override void Die()
    {
        base.Die();
        HouseManager.Instance.DestroyHouseInList(this);
    }
}
