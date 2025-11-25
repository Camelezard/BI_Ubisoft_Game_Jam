using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCanon : Singleton<PlayerCanon>
{
    [SerializeField] private LayerMask _cameraRayLayer;
    [SerializeField] private float _rotationCap = 90f;
    [SerializeField] private float _maxRotationSpeed = 90f;
    [SerializeField] private GameObject _windZone;
    
    public Transform testobject;
    
    private Camera _camera;
    private InputAction _windZoneInput;
    private const string WIND_ZONE_ACTION = "WindZone";
    private FlowManager _flowManager;
    
    private void Start()
    {
        _camera = Camera.main;
        DisableWindZone();
        _windZoneInput = InputManager.instance.GetInputAction(WIND_ZONE_ACTION);
        _windZoneInput.performed += ctx => EnableWindZone();
        _windZoneInput.canceled += ctx => DisableWindZone();
        _flowManager = FlowManager.instance;
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
        
        if(Physics.Raycast(lRay, out RaycastHit lHit, Mathf.Infinity, _cameraRayLayer))
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
    
    private void EnableWindZone()
    {
        if(!_flowManager.IsPlaying) return;
        _windZone.SetActive(true);
    }
    
    private void DisableWindZone()
    {
        _windZone.SetActive(false);
    }
    
    private void OnDestroy()
    {
        _windZoneInput.performed -= ctx => EnableWindZone();
        _windZoneInput.canceled -= ctx => DisableWindZone();
    }
}
