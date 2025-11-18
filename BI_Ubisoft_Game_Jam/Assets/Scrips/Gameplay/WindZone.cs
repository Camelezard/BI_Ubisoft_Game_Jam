using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField] private LayerMask _tornadoLayer;
    
    private void OnTriggerEnter(Collider pOther)
    {
        if(pOther.TryGetComponent(out Tornado lTornado))
        {
            print("tornade détectée : " + lTornado.name);
        }
    }
}
