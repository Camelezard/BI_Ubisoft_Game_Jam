using FMODUnity;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    //public EventReference fmodEmitter = new EventReference();


    // [SerializeField] string DefeatMusic;
    // [SerializeField] string WinMusic;
    [Header("music")]
    [SerializeField] public EventReference winMusic;
    [SerializeField] public EventReference levelMusic;
    [SerializeField] public EventReference MenuMusic;
    [SerializeField] public EventReference loseMusic;

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

    private EventInstance musicInstance;

    protected override void Awake()
    {
        base.Awake();
        PlayMusic(winMusic);
        if (SceneManager.GetActiveScene().buildIndex == 0) ChangeMenuMusic();
    }

    private void PlayMusic(EventReference musicEvent)
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    public void ChangeMenuMusic()
    {
        PlayMusic(MenuMusic);
    }
    public void ChangeLevelMusic()
    {
        PlayMusic(levelMusic);
    }
    public void ChangeWinMusic()
    {
        PlayMusic(winMusic);
    }

    public void ChangeDefeatMusic()
    {
        PlayMusic(loseMusic);
    }

    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }
}
