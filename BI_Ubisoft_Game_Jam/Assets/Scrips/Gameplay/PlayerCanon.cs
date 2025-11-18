using UnityEngine;

public class PlayerCanon : MonoBehaviour
{
    [SerializeField] private LayerMask _cameraRayLayer;
    [SerializeField] private float _rotationCap = 90f;
    [SerializeField] private float _maxRotationSpeed = 90f;
    
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
        Vector3 lDirection;
        // Vector3 lMousePos = Input.mousePosition;
        // lMousePos.z = Vector3.Distance(_camera.transform.position, transform.position);
        // lMousePos = _camera.ScreenToWorldPoint(lMousePos);
        // testobject.position = lMousePos;
        
        if(Physics.Raycast(lRay, out RaycastHit lHit))
        {
            testobject.position = lHit.point;
            // transform.rotation = Quaternion.LookRotation(
            //     Vector3.ProjectOnPlane(lHit.point - transform.position, Vector3.up), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(
                Vector3.ProjectOnPlane(lHit.point - transform.position, Vector3.up), Vector3.up), _maxRotationSpeed * Time.deltaTime);
                
            lLimitedAngle = Vector3.SignedAngle(transform.parent.forward, transform.forward, Vector3.up);
            lLimitedAngle = Mathf.Clamp(lLimitedAngle, -_rotationCap, _rotationCap);
            lDirection = Quaternion.AngleAxis(lLimitedAngle, Vector3.up) * transform.parent.forward;
            // transform.rotation = Quaternion.LookRotation(lRotation, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(lDirection, Vector3.up), _maxRotationSpeed * Time.deltaTime);
        }
    }
}
