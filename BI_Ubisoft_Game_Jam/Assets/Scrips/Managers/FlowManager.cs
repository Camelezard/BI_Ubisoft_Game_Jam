using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FlowEvent
{
    public ScriptableObject eventObject;
    public float timeBeforeNextEvent = 1f;
}

public class FlowManager : MonoBehaviour
{
    [SerializeField] private List<FlowEvent> _eventList = new();
    
    [Header("Testing")]
    [SerializeField] private bool _launchFlowOnStart = true;
    
    [Tooltip("If you want to start the flow at a different index than first event.")]
    [SerializeField] private int _firstEventIndex = 0;
    
    private int _currentEventIndex = -1;
    
    private bool _isPlaying = false;
    public bool IsPlaying{get => _isPlaying;}
    
    private const string DIALOG_SO = "DialogSO",
                        TORNADO_DATA = "TornadoData";
    
    #region singleton
    
    private static FlowManager _Instance;
    public static FlowManager instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no FlowManager instance found");
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
            Debug.Log("FlowManager already exists");
        }
    }
    
    #endregion
    
    void Start()
    {
        // for (int i = 0; i < _eventList.Count; i++)
        // {
        //     print(_eventList[i].eventObject.GetType());
        // }
        
        if(_firstEventIndex > 0) _currentEventIndex = _firstEventIndex - 1;
        if(_firstEventIndex < -1) _firstEventIndex = -1;
        if(_launchFlowOnStart) LaunchNextFlowEvent();
        DialogManager.OnDialogOver += OnEventEnd;
        TornadoWaveManager.OnWaveEnd += OnEventEnd;
    }
    
    private void LaunchNextFlowEvent()
    {
        _currentEventIndex++;
        if (_currentEventIndex > _eventList.Count - 1)
        {
            Debug.Log("Fin des événements du FlowManager");
            return;
        }
        
        FlowEvent lEvent = _eventList[_currentEventIndex];
        
        if (lEvent.eventObject == null)
        {
            Debug.LogError("SO vide dans la liste d'event du FlowManager, index " + _currentEventIndex);
            return;
        }
        
        switch (lEvent.eventObject.GetType().ToString())
        {
            case DIALOG_SO:
                // print("c'est un dialog");
                DialogManager.instance.LaunchDialogSO(lEvent.eventObject as DialogSO);
                Time.timeScale = 0f;
                _isPlaying = false;
                break;
            case TORNADO_DATA:
                // print("c'est une tornadodata");
                //iyhttg
                TornadoWaveManager.instance.LaunchWaveEvent(lEvent.eventObject as TornadoData);
                Time.timeScale = 1f;
                _isPlaying = true;
                break;
            default:
                break;
        }
    }
    
    private IEnumerator NextEventCoroutine()
    {
        float lElapsedTime = 0f;
        
        while (lElapsedTime < _eventList[_currentEventIndex].timeBeforeNextEvent)
        {
            lElapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        LaunchNextFlowEvent();
    }
    
    private void OnEventEnd()
    {
        StartCoroutine(NextEventCoroutine());
    }
    
    private void OnDestroy()
    {
        DialogManager.OnDialogOver -= OnEventEnd;
        TornadoWaveManager.OnWaveEnd -= OnEventEnd;
    }
}
