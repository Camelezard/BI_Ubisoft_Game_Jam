using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _container;
    [SerializeField] private TMP_Text _dialogNameLeftText;
    [SerializeField] private TMP_Text _dialogNameRightText; 
    [SerializeField] private TMP_Text _dialogBoxText;
    [SerializeField] private Image _backgroundPanel, _leftCharacterSprite, _rightCharacterSprite;
    
    [Header("Variables")]
    [SerializeField] private float _dialogUIAppearTime = 0.3f;
    [SerializeField] private float _backgroundPanelAlpha = 0.39f;
    [SerializeField] private int _dialogTextSpeed = 15;
    
    [SerializeField] private DialogSO _testDialog;
    
    private Coroutine _coroutineDialogUI;
    
    #region singleton

    private static DialogManager _Instance;
    public static DialogManager instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no DialogManager instance found");
                return null;
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("DialogManager already exists");
        }
    }
    
    #endregion
    
    private void Start()
    {
        _container.alpha = 0f;
        _dialogBoxText.text = string.Empty;
        LaunchDialogSO(_testDialog);
    }
    
    public void LaunchDialogSO(DialogSO pDialog)
    {
        if(pDialog == null)
        {
            Debug.LogError("DialogSO null reference");
            return;
        }
        
        EraseCoroutine(_coroutineDialogUI);
        _coroutineDialogUI = StartCoroutine(DialogUIAppear());
    }
    
    private IEnumerator DialogUIAppear()
    {
        float lElapsedTime = 0f;
        
        while (lElapsedTime < _dialogUIAppearTime)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            _container.alpha = lElapsedTime / _dialogUIAppearTime;
            yield return new WaitForEndOfFrame();
        }
        
        _container.alpha = 1f;
        
        yield return null;
    }
    
    private void EraseCoroutine(Coroutine pCoroutine)
    {
        if(pCoroutine != null) StopCoroutine(pCoroutine);
        pCoroutine = null;
    }
}
