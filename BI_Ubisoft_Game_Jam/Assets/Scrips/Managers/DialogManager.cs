using System;
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
    [SerializeField] private Image _dialogNameLeft;
    [SerializeField] private Image _dialogNameRight;
    [SerializeField] private TMP_Text _dialogBoxText;
    [SerializeField] private Image _backgroundPanel, _leftCharacterSprite, _rightCharacterSprite;
    
    [Header("Variables")]
    [SerializeField] private float _dialogUIAppearTime = 0.3f;
    [SerializeField] private float _backgroundPanelAlpha = 0.39f;
    [SerializeField] private int _dialogTextSpeed = 15;
    
    [Header("Game Feel")]
    [SerializeField] private Color _baseCharColor;
    [SerializeField] private Color _notTalkingCharColor;
    [SerializeField, Range(1f, 1.5f)] private float _talkingScaleIncrease = 1.1f;
    [SerializeField] private float _talkingScaleIncreaseTime = 0.3f;
    
    [Header("Resources")]
    [SerializeField] private Sprite _sethSprite;
    [SerializeField] private Sprite _osirisSprite;
    
    [SerializeField] private DialogSO _testDialog;
    
    private Coroutine _coroutineDialogUI;
    private Coroutine _coroutineDialogText;
    private Coroutine _coroutineCharacterTalk;
    private DialogSO _currentDialogSO;
    private int _dialogIndex = -1;
    private bool _currentDialogOver = false;
    
    private const string DIALOG_FORWARD = "DialogForward";
    private InputAction _dialogForward;
    
    public static event Action OnDialogOver;
    
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
        _dialogForward = InputManager.instance.GetInputAction(DIALOG_FORWARD);
    }
    
    #endregion
    
    private void Start()
    {
        _container.alpha = 0f;
        _dialogBoxText.text = string.Empty;
        // _dialogForward = InputManager.instance.GetInputAction(DIALOG_FORWARD);
        // LaunchDialogSO(_testDialog);
    }
    
    public void LaunchDialogSO(DialogSO pDialog)
    {
        if(pDialog == null)
        {
            Debug.LogError("DialogSO null reference");
            return;
        }
        
        _dialogForward.performed += ctx => OnDialogForward();
        
        _currentDialogSO = pDialog;
        ManageCharacterObjects();
        
        EraseCoroutine(_coroutineDialogUI);
        _coroutineDialogUI = StartCoroutine(DialogUIAppear());
    }
    
    private void EndDialogSO()
    {
        EraseCoroutine(_coroutineDialogUI);
        _coroutineDialogUI = StartCoroutine(DialogUIDisappear());
        _currentDialogSO = null;
        
        _dialogForward.performed -= ctx => OnDialogForward();
    }
    
    private IEnumerator DialogUIAppear()
    {
        float lElapsedTime = 0f;
        
        ResizeCharacterSprites();
        
        _dialogIndex = -1;
        
        EraseCoroutine(_coroutineCharacterTalk);
        _coroutineCharacterTalk = StartCoroutine(CharacterTalkVisualCoroutine());
        _dialogBoxText.text = string.Empty;
        
        while (lElapsedTime < _dialogUIAppearTime)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            _container.alpha = lElapsedTime / _dialogUIAppearTime;
            yield return new WaitForEndOfFrame();
        }
        
        _container.alpha = 1f;
        
        // _dialogIndex = -1;
        
        EraseCoroutine(_coroutineDialogText);
        _coroutineDialogText = StartCoroutine(DialogTextAppear());
        
        yield return null;
    }
    
    private IEnumerator DialogUIDisappear()
    {
        float lElapsedTime = 0f;
        
        while (lElapsedTime < _dialogUIAppearTime)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            _container.alpha = 1f - lElapsedTime / _dialogUIAppearTime;
            yield return new WaitForEndOfFrame();
        }
        
        _container.alpha = 0f;
        OnDialogOver?.Invoke();
        
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
        
        _currentDialogOver = false;
        _dialogIndex++;
        float lElapsedTime = 0f;
        int lTotalTextLength = _currentDialogSO.dialogList[_dialogIndex].dialog.Length;
        float lTotalTime = lTotalTextLength / _dialogTextSpeed;
        int lTextLength;
        string lCurrentText;
        
        EraseCoroutine(_coroutineCharacterTalk);
        _coroutineCharacterTalk = StartCoroutine(CharacterTalkVisualCoroutine());
        
        while (lElapsedTime < lTotalTime)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            lTextLength = Mathf.FloorToInt(lTotalTextLength * (lElapsedTime / lTotalTime));
            lCurrentText = string.Empty;
            
            for (int i = 0; i < lTextLength; i++)
            {
                lCurrentText += _currentDialogSO.dialogList[_dialogIndex].dialog[i];
                _dialogBoxText.text = lCurrentText;
            }
            
            yield return new WaitForEndOfFrame();
        }
        
        _currentDialogOver = true;
        
        yield return null;
    }
    
    private IEnumerator CharacterTalkVisualCoroutine()
    {
        float lElapsedTime = 0f;
        int lDialogIndex = _dialogIndex < 0 ? 0 : _dialogIndex;
        Image lIncreasingSprite = _currentDialogSO.dialogList[lDialogIndex].characterSide == CharacterSide.leftCharacter ? _leftCharacterSprite : _rightCharacterSprite;
        Image lDecreasingSprite = _currentDialogSO.dialogList[lDialogIndex].characterSide == CharacterSide.leftCharacter ? _rightCharacterSprite : _leftCharacterSprite;
        
        Image lIncreasingName = _currentDialogSO.dialogList[lDialogIndex].characterSide == CharacterSide.leftCharacter ? _dialogNameLeft : _dialogNameRight;
        Image lDecreasingName = _currentDialogSO.dialogList[lDialogIndex].characterSide == CharacterSide.leftCharacter ? _dialogNameRight : _dialogNameLeft;
        
        float lRatio;
        Vector3 lIncreasingSpriteBaseScale = lIncreasingSprite.transform.localScale;
        Vector3 lDecreasingSpriteBaseScale = lDecreasingSprite.transform.localScale;
        Color lIncreasingSpriteBaseColor = lIncreasingSprite.color;
        Color lDecreasingSpriteBaseColor = lDecreasingSprite.color;
        
        while (lElapsedTime < _talkingScaleIncreaseTime)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            
            lRatio = lElapsedTime / _talkingScaleIncreaseTime;
            lIncreasingSprite.transform.localScale = Vector3.Lerp(lIncreasingSpriteBaseScale, Vector3.one * _talkingScaleIncrease, lRatio);
            lIncreasingSprite.color = lIncreasingName.color = Color.Lerp(lIncreasingSpriteBaseColor, _baseCharColor, lRatio);
            
            lDecreasingSprite.transform.localScale = Vector3.Lerp(lDecreasingSpriteBaseScale, Vector3.one, lRatio);
            lDecreasingSprite.color = lDecreasingName.color = Color.Lerp(lDecreasingSpriteBaseColor, _notTalkingCharColor, lRatio);
            
            yield return new WaitForEndOfFrame();
        }
        
    }
    
    private void OnDialogForward()
    {
        if(_currentDialogSO == null ||_coroutineDialogText == null) return;
        EraseCoroutine(_coroutineDialogText);
        if(!_currentDialogOver)
        {
            _dialogBoxText.text = _currentDialogSO.dialogList[_dialogIndex].dialog;
            _currentDialogOver = true;
        }
        else
        {
            if(_dialogIndex < _currentDialogSO.dialogList.Count - 1) _coroutineDialogText = StartCoroutine(DialogTextAppear());
            else EndDialogSO();
        }
    }
    
    private void ManageCharacterObjects()
    {
        if(_currentDialogSO.leftCharacter != CharacterNames.None)
        {
            _dialogNameLeftText.transform.parent.gameObject.SetActive(true);
            _dialogNameLeftText.text = _currentDialogSO.leftCharacter.ToString();
            _leftCharacterSprite.gameObject.SetActive(true);
            SetCharacterSprite(_leftCharacterSprite, _currentDialogSO.leftCharacter);
        }
        else
        {
            _leftCharacterSprite.gameObject.SetActive(false);
            _dialogNameLeftText.transform.parent.gameObject.SetActive(false);
        }
        
        if(_currentDialogSO.rightCharacter != CharacterNames.None)
        {
            _rightCharacterSprite.gameObject.SetActive(true);
            _dialogNameRightText.transform.parent.gameObject.SetActive(true);
            _dialogNameRightText.text = _currentDialogSO.rightCharacter.ToString();
            SetCharacterSprite(_rightCharacterSprite, _currentDialogSO.rightCharacter);
        }
        else
        {
            _rightCharacterSprite.gameObject.SetActive(false);
            _dialogNameRightText.transform.parent.gameObject.SetActive(false);
        }
    }
    
    private void ResizeCharacterSprites()
    {
        _leftCharacterSprite.transform.localScale = _rightCharacterSprite.transform.localScale = Vector3.one;
        _leftCharacterSprite.color = _rightCharacterSprite.color = _dialogNameLeft.color = _dialogNameRight.color = _baseCharColor;
    }
    
    private void SetCharacterSprite(Image pImage, CharacterNames pName)
    {
        switch (pName)
        {
            case CharacterNames.Seth :
                pImage.sprite = _sethSprite;
                break;
            case CharacterNames.Osiris :
                pImage.sprite = _osirisSprite;
                break;
            default:
                break;
        }
    }
    
    private void EraseCoroutine(Coroutine pCoroutine)
    {
        if(pCoroutine != null) StopCoroutine(pCoroutine);
        pCoroutine = null;
    }
    
    private void OnDestroy()
    {
        _dialogForward.performed -= ctx => OnDialogForward();
    }
}
