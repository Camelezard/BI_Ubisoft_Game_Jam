using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Tornado : MonoBehaviour
{
    [SerializeField] private float _TornadoSpeed = 10;
    [SerializeField] private float _TornadoDamamges = 10;
    [SerializeField] private float _TornadoWeight = 10;
    [SerializeField] private float _TornadoStartLitime = 10;
    private float _TornadoLitime = 10;
    [SerializeField] public bool _CanPassAWall = true;
    [SerializeField] public bool _ChooseTotalRendomDirection = false;
    public Vector3 _Direction;

    [SerializeField] private LayerMask _LayerToIgnior;
    [SerializeField] private MeshCollider _MeshCollider;
    void Awake()
    {
        int tornadoLayer = LayerMask.NameToLayer("Tornados");
        Physics.IgnoreLayerCollision(tornadoLayer, tornadoLayer);
    }

    void Start()
    {
        ChooseInitialDirection();
        InitLifetime();

        if(!_MeshCollider) _MeshCollider =  GetComponent<MeshCollider>();
    }

    private void Update()
    {
        transform.position += _Direction * _TornadoSpeed * Time.deltaTime;

        if (_TornadoLitime > 0) _TornadoLitime -= Time.deltaTime;
        else EndLifTime();
    }

    private void EndLifTime()
    {
        Destroy(gameObject);
    }

    private void InitLifetime()
    {
        _TornadoLitime = _TornadoStartLitime;
    }

    private void ChooseInitialDirection()
    {
        Vector2 _Circle = Random.insideUnitCircle.normalized;
        if(_ChooseTotalRendomDirection) _Direction = new Vector3(_Circle.x, 0, _Circle.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (_CanPassAWall)
            {   
                _MeshCollider.isTrigger = true;

                StartCoroutine(EnableWallCollisionAfterDelay());
                return;
            }

            if (_CanPassAWall) return;

            Vector3 normal = collision.contacts[0].normal;
            _Direction = Vector3.Reflect(_Direction, normal);
            _Direction.y = 0f;
        }
    }

    private IEnumerator EnableWallCollisionAfterDelay() 
    {
        yield return new WaitForSeconds(5);
        _CanPassAWall = false; 
        _MeshCollider.isTrigger = false;
    }

    private void OnTriggerStay(Collider other)
    {
        House l_House = other.GetComponent<House>();
        if (l_House)
        {
            if (l_House != null)
            {
                l_House.TakeDamage(_TornadoDamamges * Time.deltaTime);
            }
        }
    }
}