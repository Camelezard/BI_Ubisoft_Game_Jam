using UnityEngine;

public class HUD : MonoBehaviour
{
    #region singleton
    
    private static HUD _Instance;
    public static HUD instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no HUD instance found");
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
            Debug.Log("HUD already exists");
        }
    }
    
    #endregion
}
