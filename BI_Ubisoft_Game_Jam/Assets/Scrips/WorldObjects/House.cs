using UnityEngine;

public class House : WorldObject
{
    [Header("Gold")] public int goldGainOnWaveEnd;
    
    [SerializeField] GameObject corp = null;
    [SerializeField] GameObject ruine = null;
    [SerializeField] float _DestroyFallAmont = 7f;
    protected override void Die()
    {
        base.Die();
        HouseManager.Instance.DestroyHouseInList(this);

        ShowDestroyAspect();
    }

    private void ShowDestroyAspect()
    {
        if(corp) corp.transform.position += Vector3.down * _DestroyFallAmont;

        if(ruine) ruine.SetActive(false);
    }
}
