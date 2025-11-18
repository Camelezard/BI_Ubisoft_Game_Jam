using UnityEngine;

public class House : WorldObject
{
    protected override void Die()
    {
        base.Die();
        //WorldObjectSpawner.Instance.RemoveHouse(this);
    }
}
