using FMODUnity;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Player")]
    [SerializeField] public EventReference playerWind;
    [SerializeField] public EventReference playerMoove;

    [Header("houses")]
    [SerializeField] public EventReference houseConstruct;
    [SerializeField] public EventReference houseDestroy;
    [SerializeField] public EventReference houseTakeDamages;
    [SerializeField] public EventReference houseHeal;

    [Header("tornado")]
    [SerializeField] public EventReference torandoBounce;
    [SerializeField] public EventReference torandoAppear;
    [SerializeField] public EventReference torandoDisappear;

    [Header("mony")]
    [SerializeField] public EventReference loseMony;
    [SerializeField] public EventReference reciveMony;

    [Header("other")]
    [SerializeField] public EventReference loseSond;
    [SerializeField] public EventReference winSond;

    public void Test()
    {

    }
}
