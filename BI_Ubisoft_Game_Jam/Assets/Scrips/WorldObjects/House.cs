using System.Collections;
using System;
using UnityEngine;
using Unity.Mathematics;

public class House : WorldObject
{

    [SerializeField] GameObject corp = null;
    [SerializeField] GameObject ruine = null;
    [SerializeField] GameObject UI_object = null;
    [SerializeField] float _DestroyFallAmont = 7f;
    [Header("Gold")] public int goldGainOnWaveEnd;

    private float _shakeSpeed = 1;
    [SerializeField] float shakeSpeed = 30f;
    private Vector3 pHomePos;

    private bool IsShaking = false;

    public override void Start()
    {
        pHomePos = corp.transform.localPosition;
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        HouseManager.Instance.DestroyHouseInList(this);

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.houseDestroy);

        ShowDestroyAspect();
        
        
        UI_object.SetActive(false);
    }

    private void ShowDestroyAspect()
    {
        if (corp) corp.SetActive(false);

        if (ruine) ruine.SetActive(true);

        

        isDestroyed = true;
    }

    public void HouseShake()
    {
        if (!IsShaking) StartCoroutine(Shake());

    }

    private IEnumerator Shake()
    {
        if (IsShaking || isDestroyed) yield break;
        IsShaking = true;

        Vector2 randCircle = UnityEngine.Random.insideUnitCircle * 1f;
        Vector3 targetPos = pHomePos + new Vector3(randCircle.x, 0, randCircle.y);

        while (Vector3.Distance(targetPos, corp.transform.localPosition) > 0.1f && !isDestroyed)
        {
            corp.transform.localPosition = Vector3.MoveTowards(
                corp.transform.localPosition,
                targetPos,
                shakeSpeed * Time.deltaTime
            );

            yield return null;
        }

        corp.transform.localPosition = pHomePos;

        IsShaking = false;
    }
}
