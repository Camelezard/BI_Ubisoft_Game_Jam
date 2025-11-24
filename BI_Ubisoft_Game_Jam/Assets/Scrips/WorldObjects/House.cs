using UnityEngine;

public class House : WorldObject
{
    protected override void Die()
    {
        base.Die();
        HouseManager.Instance.DestroyHouseInList(this);
    }
}
