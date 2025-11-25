using NUnit.Framework;
using UnityEngine;

public class House : WorldObject
{
    [SerializeField] private GameObject _Corp = null;
    [SerializeField] private GameObject _Ruins = null;

    public bool isDestroy {get; private set;} = false;

    public override void Start()
    {
        base.Start();

        _Corp.SetActive(true);
        _Ruins.SetActive(false);
    }

    protected override void Die()
    {
        //base.Die();
        print("death");

        HouseManager.Instance.DestroyHouseInList(this);

        isDestroy = true;

        _Corp.SetActive(false);
        _Ruins.SetActive(true);

        _HP_Field.gameObject.SetActive(false);
    }
}
