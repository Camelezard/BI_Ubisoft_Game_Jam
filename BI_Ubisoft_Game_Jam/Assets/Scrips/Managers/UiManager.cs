using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;
public class UiManager : Singleton<UiManager>
{
    public static event Action OnVictory;
    public static event Action OnDefeat;

    [Header("Panels")]
    [SerializeField] private GameObject _PanelContainer;
    [SerializeField] private GameObject _PanelDefeat;
    [SerializeField] private GameObject _PanelWin;
    [SerializeField] private GameObject _MenuPanel;
    [SerializeField] private GameObject _PausePanel;
    [SerializeField] private GameObject _CreditsPanel;
    [SerializeField] private GameObject _SettingsPanel;
    [SerializeField] private GameObject _LevelSelector;
    [SerializeField] private GameObject _GameUi;
    private GameObject _ActifPanel = null;
    [SerializeField] private List<GameObject> _PreviusPanel = new List<GameObject>();

    //[SerializeField] Slider _destruction_Slider;
    [SerializeField] Slider _Wave_Slider;

    [Header("imageGenante")]
    [SerializeField] private List<Image> warningImages;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float disappearDelay = 1f;

    private bool _isGamePaused = false;

    protected virtual void Start()
    {
        CheckShowPanel();
        OnDefeat += Defeat;
        OnVictory += Defeat;
        if (InputManager.instance != null) InputManager.instance.GetInputAction("Pause").performed += ctx => SwapPause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void CheckShowPanel()
    {
        foreach (Transform child in _PanelContainer.transform)
        {
            GameObject lCheckedPanel = child.gameObject;

            if (lCheckedPanel.TryGetComponent(out DialogManager lDialogManager))
            {
                lDialogManager.gameObject.SetActive(true);
                continue;
            }

            if (_ActifPanel == null && lCheckedPanel.activeInHierarchy)
            {
                _ActifPanel = lCheckedPanel;
            }
            else
            {
                lCheckedPanel.SetActive(false);
            }
        }

        if (_ActifPanel == null) _ActifPanel = _MenuPanel;
        _ActifPanel.SetActive(true);
        SoundManager.Instance.ChangeMenuMusic();

        //
    }

    private void ChangePannel(GameObject pNewPanel, bool pRememberPanel = true)
    {
        if (_ActifPanel != pNewPanel)
        {
            _ActifPanel.SetActive(false);
            if (pRememberPanel) _PreviusPanel.Add(_ActifPanel);

            _ActifPanel = pNewPanel;
            _ActifPanel.SetActive(true);
        }
    }

    public void PanelBack()
    {
        if (_PreviusPanel.Count > 0)
        {
            GameObject lastPanel = _PreviusPanel.Last();
            _PreviusPanel.RemoveAt(_PreviusPanel.Count - 1);
            ChangePannel(lastPanel, false);
        }
    }

    public void SwapPause()
    {
        print("pause");

        if (SceneManager.GetActiveScene().buildIndex != 0) SetPause(_isGamePaused = !_isGamePaused);
        else ShowMenu();
    }

    public void SetPause(bool pState = true)
    {
        _isGamePaused = pState;
        Time.timeScale = pState ? 0f : 1f;

        if (pState)
        {
            _PausePanel.SetActive(true);
            _ActifPanel = _PausePanel;
        }
        else
        {
            HideCurrnetPanel();
        }
    }


    // Show Panels
    public void HideCurrnetPanel()
    {
        _ActifPanel.SetActive(false);
    }

    public void ShowMenu()
    {
        ChangePannel(_MenuPanel);
    }

    public void ShowSettings()
    {
        ChangePannel(_SettingsPanel);
    }

    public void ShowCredits()
    {
        ChangePannel(_CreditsPanel);
    }

    public void ShowPause()
    {
        ChangePannel(_PausePanel);
    }

    public void ShowGameUi()
    {
        ChangePannel(_GameUi);
    }

    public void ShowDefeat()
    {
        //SceneManager.LoadScene(2);

        ChangePannel(_PanelDefeat);
        //print("Show Defeat");
    }


    public void ShowWin()
    {
        ChangePannel(_PanelWin);
        //SceneManager.LoadScene(3);
    }
    public void UpdateDestroyUi(int number)
    {
        //if(_destruction_Slider) _destruction_Slider.value = pPercentage;
    }

    public void UpdateWaveUi(float pPercentage)
    {
        if (_Wave_Slider) _Wave_Slider.value = pPercentage;
    }

    public IEnumerator LunshSlider(float pSliderTime)
    {
        float elapsedTime = 0;

        while (elapsedTime < pSliderTime)
        {
            elapsedTime += Time.deltaTime;

            UpdateWaveUi(elapsedTime / pSliderTime);
            yield return null;
        }
    }

    //Load levels
    public void ReturnToMenu()
    {
        LoadGameLevel(0);
        //SoundManager.Instance.StopMusic();
        SoundManager.Instance.ChangeMenuMusic();
        //SoundManager.chan
        ShowMenu();
    }

    public void Win()
    {
        ShowWin();

        Time.timeScale = 0;
    }
    public void Defeat()
    {
        ShowDefeat();
        Time.timeScale = 0;
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.winSond);
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.loseMusic);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PhaseSwitch", 0);
        SoundManager.Instance.ChangeDefeatMusic();
    }

    public void Victory()
    {
        ShowWin();
        Time.timeScale = 0;
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.loseSond);
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.winMusic);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PhaseSwitch", 0);
        SoundManager.Instance.ChangeWinMusic();
    }

    public void LoadGameLevel(int pLevelIndex)
    {
        HideCurrnetPanel();
        SetPause(false);
        SceneManager.LoadScene(pLevelIndex);
        SoundManager.Instance.ChangeLevelMusic();
        ShowGameUi();
    }

    public void TriggerDefeat()
    {
        OnDefeat?.Invoke();
        Debug.Log("TriggerDefeat");
        FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.loseSond);
    }

    private void OnUiDefeat()
    {
        ShowDefeat();
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SoundManager.Instance.ChangeLevelMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ShowMenu();
    }

    public IEnumerator FadeInObstructionImages()
    {
        bool allVisible = false;
        while (!allVisible)
        {
            allVisible = true;

            foreach (Image img in warningImages)
            {
                float alpha = img.color.a;
                alpha += Time.deltaTime * fadeSpeed;
                alpha = Mathf.Clamp01(alpha);
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

                if (alpha < 0.99f) allVisible = false;
            }

            yield return null;
        }
    }

    public IEnumerator FadeOutAfterObstructionDelay()
    {
        float timer = 0f;
        while (timer < disappearDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        bool allInvisible = false;
        while (!allInvisible)
        {
            allInvisible = true;

            foreach (Image img in warningImages)
            {
                float alpha = img.color.a;
                alpha -= Time.deltaTime * fadeSpeed;
                alpha = Mathf.Clamp01(alpha);
                img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

                if (alpha > 0.01f) allInvisible = false;
            }

            yield return null;
        }
    }



}
