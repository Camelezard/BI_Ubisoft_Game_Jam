using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
public class UiManager : SingletonPersistent<UiManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject _PanelContainer;
    [SerializeField] private GameObject _MenuPanel;
    [SerializeField] private GameObject _PausePanel;
    [SerializeField] private GameObject _CreditsPanel;
    [SerializeField] private GameObject _SettingsPanel;
    [SerializeField] private GameObject _LevelSelector;
    [SerializeField] private GameObject _GameUi;
    private GameObject _ActifPanel = null;
    [SerializeField] private List<GameObject> _PreviusPanel = new List<GameObject>();

    [SerializeField] Slider _destruction_Slider;
    
    private bool _isGamePaused = false;

    protected virtual void Start()
    {
        CheckShowPanel();
        InputManager.instance.GetInputAction("Pause").performed += ctx => SwapPause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SwapPause();
        
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
        if (SceneManager.GetActiveScene().buildIndex  != 0) SetPause(_isGamePaused = !_isGamePaused);
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

    //Load levels
    public void ReturnToMenu()
    {
        LoadGameLevel(0);
        ShowMenu();
    }

    public void LoadGameLevel(int pLevelIndex)
    {
        HideCurrnetPanel();
        SetPause(false);
        SceneManager.LoadScene(pLevelIndex);
        ShowGameUi();
    }
}
