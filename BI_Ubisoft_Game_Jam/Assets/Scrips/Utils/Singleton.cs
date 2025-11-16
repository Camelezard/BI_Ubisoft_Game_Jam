using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : Component
{
    private static T m_Instance;

    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if(m_Instance == null)
        {
            Instance = this as T;
        }
        else Destroy(gameObject);
    }

    protected virtual void OnInstanceSet() {}
}

public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
