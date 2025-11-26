using FMODUnity;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] public EventReference playerWind;
    [SerializeField] public EventReference houseConstruct;
    [SerializeField] public EventReference houseDestroy;
    [SerializeField] public EventReference takeDamages;
    public void Test()
    {
        
    }
}
