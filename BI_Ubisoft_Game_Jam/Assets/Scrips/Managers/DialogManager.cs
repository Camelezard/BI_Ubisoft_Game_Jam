using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private Coroutine _coroutineDialogText;
    private DialogSO _currentDialogSO;
    private int _dialogIndex = -1;
    
    private const string DIALOG_FORWARD = "DialogForward";
    private InputAction _dialogForward;
    
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
        _dialogForward = InputManager.instance.GetInputAction(DIALOG_FORWARD);
    }
    
    public void LaunchDialogSO(DialogSO pDialog)
    {
        if(pDialog == null)
        {
            Debug.LogError("DialogSO null reference");
            return;
        }
        
        _currentDialogSO = pDialog;
        _dialogNameLeftText.text = pDialog.leftCharacter.ToString();
        _dialogNameRightText.text = pDialog.rightCharacter.ToString();
        
        EraseCoroutine(_coroutineDialogUI);
        _coroutineDialogUI = StartCoroutine(DialogUIAppear());
    }
    
    private IEnumerator DialogUIAppear()
    {
        float lElapsedTime = 0f;
        
        while (lElapsedTime < _dialogUIAppearTime)
        {
            lElapsedTime += Time.deltaTime;
            _container.alpha = lElapsedTime / _dialogUIAppearTime;
            yield return new WaitForEndOfFrame();
        }
        
        _container.alpha = 1f;
        
        _dialogIndex = -1;
        
        EraseCoroutine(_coroutineDialogText);
        _coroutineDialogText = StartCoroutine(DialogTextAppear());
        
        yield return null;
    }
    
    private IEnumerator DialogTextAppear()
    {
        if(_dialogTextSpeed <= 0f)
        {
            Debug.LogError("Dialog Text Speed is zero");
            EraseCoroutine(_coroutineDialogText);
            yield return null;
        }
        
        _dialogIndex++;
        float lElapsedTime = 0f;
        int lTotalTextLength = _currentDialogSO.dialogList[_dialogIndex].dialog.Length;
        float lTotalTime = lTotalTextLength / _dialogTextSpeed;
        int lTextLength;
        string lCurrentText;
        
        while (lElapsedTime < lTotalTime)
        {
            lElapsedTime += Time.deltaTime;
            lTextLength = Mathf.FloorToInt(lTotalTextLength * (lElapsedTime / lTotalTime));
            lCurrentText = string.Empty;
            
            for (int i = 0; i < lTextLength; i++)
            {
                lCurrentText += _currentDialogSO.dialogList[_dialogIndex].dialog[i];
                _dialogBoxText.text = lCurrentText;
            }
            
            yield return new WaitForEndOfFrame();
        }
        
        yield return null;
    }
    
    private void EraseCoroutine(Coroutine pCoroutine)
    {
        if(pCoroutine != null) StopCoroutine(pCoroutine);
        pCoroutine = null;
    }
}
