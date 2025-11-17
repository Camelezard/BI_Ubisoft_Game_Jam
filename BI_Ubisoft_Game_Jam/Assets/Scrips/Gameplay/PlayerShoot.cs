using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private LayerMask _layer;
    public Transform testobject;
    
    private Camera _camera;
    
    private void Start()
    {
        _camera = Camera.main;
    }
    
    void Update()
    {
        Ray lRay = _camera.ScreenPointToRay(Input.mousePosition);
        // Vector3 lMousePos = Input.mousePosition;
        // lMousePos.z = Vector3.Distance(_camera.transform.position, transform.position);
        // lMousePos = _camera.ScreenToWorldPoint(lMousePos);
        // testobject.position = lMousePos;
        
        if(Physics.Raycast(lRay, out RaycastHit lHit))
        {
            testobject.position = lHit.point;
        }
    }
}
