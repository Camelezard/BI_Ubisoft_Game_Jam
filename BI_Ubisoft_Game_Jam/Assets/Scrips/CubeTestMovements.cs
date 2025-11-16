using UnityEngine;

public class CubeTestMovements : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0f, 0f, 100f * Time.deltaTime);
    }
}
