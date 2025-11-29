using System;
using UnityEngine;

public class NextWaveButton : MonoBehaviour
{
    public static event Action OnNextWaveButton;
    
    public void ButtonPressed()
    {
        OnNextWaveButton?.Invoke();
    }
}
