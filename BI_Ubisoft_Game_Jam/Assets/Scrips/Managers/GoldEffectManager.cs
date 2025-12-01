using System;
using System.Collections.Generic;
using UnityEngine;

public class GoldEffectManager : MonoBehaviour
{
    [SerializeField] private Transform _goldDisplayPrefab;
    
    private List<GameObject> _goldDisplaysList = new(){};
    
    private void Start()
    {
        FlowManager.OnGoldGain += DisplayGoldGains;
    }
    
    private void DisplayGoldGains()
    {
        DestroyExistingDisplays();
        
        bool lGainedMoney = false;
        
        foreach (House lHouse in HouseManager.Instance._InGameHouses)
        {
            if (!lHouse.isDestroyed)
            {
                CreateDisplay(lHouse);
                lGainedMoney = true;
            }      
        }
        
        if(lGainedMoney) FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.reciveMony);
    }
    
    private void CreateDisplay(House pHouse)
    {
        GoldDisplay lDisplay = Instantiate(_goldDisplayPrefab).GetComponent<GoldDisplay>();
        lDisplay.transform.SetParent(UiManager.Instance.Canvas);
        lDisplay.transform.position = Camera.main.WorldToScreenPoint(pHouse.transform.position);
        lDisplay.StartAnim(pHouse.goldGainOnWaveEnd);
    }
    
    private void DestroyExistingDisplays()
    {
        int lCount = _goldDisplaysList.Count;
        for (int i = lCount - 1; i >= 0; i--)
        {
            Destroy(_goldDisplaysList[i]);
        }
        _goldDisplaysList.Clear();
    }
    
    private void OnDestroy()
    {
        FlowManager.OnGoldGain -= DisplayGoldGains;
    }
}
