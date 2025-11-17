using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    
    private InputSystem_Actions _playerControls;
    
    private Dictionary<string, InputAction> _inputActions = new();
    
    #region singleton

    private static InputManager _Instance;
    public static InputManager instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no InputManager instance found");
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
            Debug.Log("InputManager already exists");
        }
        
        _playerControls = new InputSystem_Actions();
        InitializeInputActions();
    }
    
    #endregion
    
    void OnEnable()
    {
        EnableAllInputs();
    }
    
    void OnDisable()
    {
        EnableAllInputs(false);
    }
    
    private void EnableAllInputs(bool pEnable = true)
    {
        if (pEnable) _playerControls.Enable();
        else _playerControls.Disable();
    }
    
    private void InitializeInputActions()
    {
        int lMapCount = _playerControls.asset.actionMaps.Count;
        int lActionsCount;
        InputActionMap lMap;
        InputAction lAction;
        
        for (int i = 0; i < lMapCount; i++)
        {
            lMap = _playerControls.asset.actionMaps[i];
            lActionsCount = lMap.actions.Count;
            for (int j = 0; j < lActionsCount; j++)
            {
                lAction = lMap.actions[j];
                _inputActions.Add(lAction.name, lAction);
            }
        }
    }
    
    public InputAction GetInputAction(string pActionName)
    {
        if(_inputActions.TryGetValue(pActionName, out InputAction lAction))
        {
            return lAction;
        }
        
        Debug.LogError("You tried to access an input action that doesn't exist");
        return null;
    }
}
