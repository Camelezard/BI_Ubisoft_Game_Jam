using UnityEngine;

public class House : WorldObject
{
    protected override void Die()
    {
        HouseManager.Instance.DestroyHouseInList(this);
        base.Die();
    }
}
