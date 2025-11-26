using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WindZone : MonoBehaviour
{
    [SerializeField] private LayerMask _tornadoLayer;
    [SerializeField] private Transform _coneTip;
    [SerializeField] private float _maxSpeedIncreasePerSec = 10f;

    private List<Tornado> _tornadosInRange = new(){};
    
    [HideInInspector] public bool _aspirate = false;
    [HideInInspector] public bool _canAspirate = false;
    
    #region singleton
    
    private static WindZone _Instance;
    public static WindZone instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.Log("no WindZone instance found");
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
            Debug.Log("WindZone already exists");
        }
        _canAspirate = false;
    }
    
    #endregion
    
    
    private void Update()
    {
        // Debug.DrawRay(transform.position, transform.parent.parent.forward * Vector3.Distance(transform.position, _coneTip.position));
        RemoveNullTornados();
        ApplyForceToTornados();
    }
    
    private void RemoveNullTornados()
    {
        int lCount = _tornadosInRange.Count;
        for (int i = lCount - 1; i >= 0; i--)
        {
            if (_tornadosInRange[i] == null) _tornadosInRange.RemoveAt(i);
        }
    }
    
    private void ApplyForceToTornados()
    {
        float lCoeff;
        Tornado lTornado;
        int lCount = _tornadosInRange.Count;
        for (int i = 0; i < lCount; i++)
        {
            lTornado = _tornadosInRange[i];
            lCoeff = 1f - Vector3.Distance(Vector3.ProjectOnPlane(transform.position, Vector3.up), 
                    Vector3.ProjectOnPlane(lTornado.transform.position, Vector3.up)) / Vector3.Distance(transform.position, _coneTip.position);
                    
            if(lCoeff < 0f)
            {
                // print("lCoeff inférieur à zero : " + lCoeff);
                continue;
            }
            
            if(!_aspirate)
            {
                
                lTornado.AddVelocity(Time.deltaTime * _maxSpeedIncreasePerSec 
                    * lCoeff * Vector3.ProjectOnPlane((lTornado.transform.position - transform.position).normalized, Vector3.up));
                
                // lTornado._coefftext.text = lCoeff.ToString("F2");
            }
            else
            {
                lCoeff = 1f - lCoeff;
                lTornado.AddVelocity(Time.deltaTime * _maxSpeedIncreasePerSec 
                    * -lCoeff * Vector3.ProjectOnPlane((lTornado.transform.position - transform.position).normalized, Vector3.up));
            }
        }
    }
    
    private void OnTriggerEnter(Collider pOther)
    {
        if(pOther.TryGetComponent(out Tornado lTornado))
        {
            //print("tornade détectée : " + lTornado.name);
            lTornado.direction = (lTornado.transform.position - PerimeterFollower.instance.transform.position).normalized;
            _tornadosInRange.Add(lTornado);
        }
    }
    
    private void OnTriggerExit(Collider pOther)
    {
        if(pOther.TryGetComponent(out Tornado lTornado) && _tornadosInRange.Contains(lTornado))
        {
            _tornadosInRange.Remove(lTornado);
        }
    }
    
    public void SetWidth(float lCoeff)
    {
        Vector3 lScale = transform.localScale;
        lScale.y *= lCoeff;
        transform.localScale = lScale;
    }
    
    public void SetLength(float lCoeff)
    {
        Vector3 lScale = transform.localScale;
        lScale.z *= lCoeff;
        transform.localScale = lScale;
    }
    
    public void SetStrength(float pCoeff)
    {
        _maxSpeedIncreasePerSec *= pCoeff;
    }
}
