using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    public static Action OnMonnyChange;
    [SerializeField] private int _StartCurency = 700;
    [SerializeField] private Text _CurencyText;
    private int _Curency = 700;
    private int _CurentBuildingCost = 0;

    public void Start()
    {
        _InitStartCurnecy();

        OnMonnyChange += UpdateMonyUi;

        OnMonnyChange.Invoke();
    }

    private void _InitStartCurnecy()
    {
        _Curency = _StartCurency;
    }

    public bool CheckMonny(int pMonnyCost)
    {
        if (pMonnyCost <= _Curency)
        {
            _CurentBuildingCost = pMonnyCost;
            return true;
        }

        return false;
    }

    public bool Buy()
    {
        if (_CurentBuildingCost <= _Curency)
        {
            _Curency -= _CurentBuildingCost;
            OnMonnyChange.Invoke();
            return true;
        }
        return false;
    }
    private void UpdateMonyUi()
    {
        _CurencyText.text = $"{_Curency} $";
    }

}
