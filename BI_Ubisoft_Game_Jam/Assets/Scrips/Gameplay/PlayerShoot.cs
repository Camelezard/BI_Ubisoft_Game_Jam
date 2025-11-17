using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private LayerMask _cameraRayLayer;
    [SerializeField] private float _maxRotation = 90f;
    
    public Transform testobject;
    
    private Camera _camera;
    
    private void Start()
    {
        _camera = Camera.main;
    }
    
    void Update()
    {
        Ray lRay = _camera.ScreenPointToRay(Input.mousePosition);
        float lLimitedAngle;
        Vector3 lRotation;
        // Vector3 lMousePos = Input.mousePosition;
        // lMousePos.z = Vector3.Distance(_camera.transform.position, transform.position);
        // lMousePos = _camera.ScreenToWorldPoint(lMousePos);
        // testobject.position = lMousePos;
        
        if(Physics.Raycast(lRay, out RaycastHit lHit))
        {
            testobject.position = lHit.point;
            transform.rotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(lHit.point - transform.position, Vector3.up), Vector3.up);
                
            lLimitedAngle = Vector3.SignedAngle(transform.parent.forward, transform.forward, Vector3.up);
            lLimitedAngle = Mathf.Clamp(lLimitedAngle, -_maxRotation, _maxRotation);
            lRotation = Quaternion.AngleAxis(lLimitedAngle, Vector3.up) * transform.parent.forward;
            transform.rotation = Quaternion.LookRotation(lRotation, Vector3.up);
        }
    }
}
