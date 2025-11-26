using System.Collections;
using System;
using UnityEngine;
using Unity.Mathematics;

public class House : WorldObject
{

    [SerializeField] GameObject corp = null;
    [SerializeField] GameObject ruine = null;
    [SerializeField] float _DestroyFallAmont = 7f;
    [Header("Gold")] public int goldGainOnWaveEnd;

    private float _shakeSpeed = 1;

    private Vector3 pHomePos;

    private bool IsShaking = false;

    public override void Start()
    {
        pHomePos = corp.transform.position;
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        HouseManager.Instance.DestroyHouseInList(this);

        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.houseDestroy);

        ShowDestroyAspect();
    }

    private void ShowDestroyAspect()
    {
        if (corp) corp.transform.position += Vector3.down * _DestroyFallAmont;

        if (ruine) ruine.SetActive(false);

        isDestroyed = true;
    }

    public void HouseShake()
    {
        if (!IsShaking) StartCoroutine(Shake());

    }

    private IEnumerator Shake()
    {

        if (IsShaking || isDestroyed) yield break;
        print("shake");


        IsShaking = true;

        Vector2 randCercle;
        Vector3 randPos;
        Vector3 lPos;
        float lElapsTime = 0;
        float lPercentage = 0;

        while (lPercentage < 1)
        {
            lPos = corp.transform.position;
            lElapsTime += Time.deltaTime;
            lPercentage = math.clamp(lPos.magnitude, 0, 1);


            randCercle = UnityEngine.Random.insideUnitCircle;
            randPos = pHomePos + new Vector3(randCercle.x, 0, randCercle.y);
            if (!isDestroyed) corp.transform.position = Vector3.Lerp(randPos, lPos, lPercentage);
            else yield return null;

            if (Vector3.Distance(randPos, lPos) != 0)

                yield return null;
        }

        IsShaking = false;
    }
}
