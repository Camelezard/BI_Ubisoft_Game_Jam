using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCanon : Singleton<PlayerCanon>
{
    [SerializeField] private LayerMask _cameraRayLayer;
    [SerializeField] private float _rotationCap = 90f;
    [SerializeField] private float _maxRotationSpeed = 90f;
    [SerializeField] private GameObject _windZone;
    
    private float _initialSpeed;
    
    private WindZone _windZoneScript;
    
    public Transform testobject;
    
    private Camera _camera;
    private InputAction _windZoneInput;
    private const string WIND_ZONE_ACTION = "WindZone";
    private FlowManager _flowManager;
    
    private InputAction _windZoneAspirateInput;
    private const string WIND_ZONE_ASPIRATE_ACTION = "WindZoneAspirate";
    
    private void Start()
    {
        _camera = Camera.main;
        DisableWindZone();
        _windZoneInput = InputManager.instance.GetInputAction(WIND_ZONE_ACTION);
        _windZoneInput.performed += ctx => EnableWindZone();
        _windZoneInput.canceled += ctx => DisableWindZone();
        _flowManager = FlowManager.instance;
        
        _windZoneScript = WindZone.instance;
        _windZoneAspirateInput = InputManager.instance.GetInputAction(WIND_ZONE_ASPIRATE_ACTION);
        _windZoneAspirateInput.performed += ctx => OnAspirateButtonPerformed();
        _windZoneAspirateInput.canceled += ctx => OnAspirateButtonCanceled();
        
        _initialSpeed = _maxRotationSpeed;
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
        if(!_flowManager.IsPlaying || Time.timeScale == 0f) return;
        _windZone.SetActive(true);
    }
    
    private void DisableWindZone()
    {
        _windZone.SetActive(false);
    }
    
    public void SetRotationSpeed(float pCoeff)
    {
        // _maxRotationSpeed *= pCoeff;
        _maxRotationSpeed += _initialSpeed * (pCoeff - 1f);
    }
    
    private void OnAspirateButtonPerformed()
    {
        if(_windZoneScript._canAspirate)
        {
            EnableWindZone();
            _windZoneScript._aspirate = true;
        }
        else _windZoneScript._aspirate = false;
    }
    
    private void OnAspirateButtonCanceled()
    {
        if(!_windZoneInput.inProgress) DisableWindZone();
        _windZoneScript._aspirate = false;
    }
    
    private void OnDestroy()
    {
        _windZoneInput.performed -= ctx => EnableWindZone();
        _windZoneInput.canceled -= ctx => DisableWindZone();
        _windZoneAspirateInput.performed -= ctx => OnAspirateButtonPerformed();
        _windZoneAspirateInput.canceled -= ctx => OnAspirateButtonCanceled();
    }
}
