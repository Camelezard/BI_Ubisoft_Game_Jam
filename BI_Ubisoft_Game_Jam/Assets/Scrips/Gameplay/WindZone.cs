using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField] private LayerMask _tornadoLayer;
    [SerializeField] private Transform _coneTip;
    [SerializeField] private float _maxSpeedIncreasePerSec = 10f;

    private List<Tornado> _tornadosInRange = new(){};
    
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
            
            // lTornado.AddVelocity(Time.deltaTime * _maxSpeedIncreasePerSec 
            //     * lCoeff * Vector3.ProjectOnPlane((lTornado.transform.position - transform.position).normalized, Vector3.up));

            // lTornado._coefftext.text = lCoeff.ToString("F2");
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
}
