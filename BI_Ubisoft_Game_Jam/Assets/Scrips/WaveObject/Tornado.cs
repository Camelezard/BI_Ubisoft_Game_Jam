using System.Collections;
using TMPro;
using UnityEngine;


public class Tornado : MonoBehaviour
{
    public float tornadoInitialSpeed = 10f;
    public float tornadoMaxSpeed = 10f;
    [SerializeField] private float _TornadoDamagePerSec = 10f;
    [SerializeField] private float _TornadoWeight = 10f;
    [SerializeField] private float _TornadoStartLifetime = 10f;
    private float _TornadoLifetime = 10f;
    [SerializeField] public bool canPassAWall = true;
    [SerializeField] public bool _ChooseTotalRandomDirection = false;
    [SerializeField] public bool _SpawnInWalls;
    [SerializeField] public Transform target;
    public Vector3 _Direction;
    public Vector3 velocity;

    [SerializeField] private LayerMask _LayerToIgnore;
    [SerializeField] private MeshCollider _MeshCollider;

    public TMP_Text _coefftext;

    void Awake()
    {
        int tornadoLayer = LayerMask.NameToLayer("Tornados");
        Physics.IgnoreLayerCollision(tornadoLayer, tornadoLayer);
    }

    void Start()
    {
        ChooseInitialDirection();
        InitLifetime();

        if (!_MeshCollider) _MeshCollider = GetComponent<MeshCollider>();
        velocity = _Direction * tornadoInitialSpeed;

        if (target == null)
        {
            target = HouseManager.Instance.RandomHouse().transform;
        }

        Vector2 lCircle2D = Random.insideUnitCircle.normalized;
        Vector3 circle = new Vector3(lCircle2D.x, 0, lCircle2D.y);

        Vector3 spawnPos = Vector3.zero;
        Vector3 dir;

        if (!_SpawnInWalls)
        {
            spawnPos = OutOfWallSpawnPosition.Instance.RndomPosOnCircle();
        }
        else
        {
            spawnPos = SpawnerManager.Instance.ChoseRandomPositinInSpawnwers();
            canPassAWall = false;
        }

        transform.position = spawnPos;

        _Direction = (target.position - spawnPos).normalized;
        velocity = _Direction * tornadoInitialSpeed;

        InitLifetime();
    }

    private void Update()
    {
        // transform.position += _TornadoSpeed * Time.deltaTime * _Direction;
        transform.position += velocity * Time.deltaTime;

        if (_TornadoLifetime > 0) _TornadoLifetime -= Time.deltaTime;
        else EndLifeTime();
    }

    private void EndLifeTime()
    {
        Destroy(gameObject);
    }

    private void InitLifetime()
    {
        _TornadoLifetime = _TornadoStartLifetime;
    }

    private void ChooseInitialDirection()
    {
        Vector2 _Circle = Random.insideUnitCircle.normalized;
        if (_ChooseTotalRandomDirection) _Direction = new Vector3(_Circle.x, 0, _Circle.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (canPassAWall)
            {
                _MeshCollider.isTrigger = true;

                StartCoroutine(EnableWallCollisionAfterDelay());
                return;
            }

            if (canPassAWall) return;

            Vector3 normal = collision.contacts[0].normal;
            velocity = Vector3.Reflect(velocity, normal);
            velocity.y = 0f;
        }
    }

    private IEnumerator EnableWallCollisionAfterDelay()
    {
        yield return new WaitForSeconds(5);
        canPassAWall = false;
        _MeshCollider.isTrigger = false;
    }

    private void OnTriggerStay(Collider other)
    {
        House l_House = other.GetComponent<House>();
        if (l_House)
        {
            if (l_House != null)
            {
                l_House.TakeDamage(_TornadoDamagePerSec * Time.deltaTime);
            }
        }
    }

    public void AddVelocity(Vector3 pForce)
    {
        velocity += pForce;
        if (velocity.magnitude > tornadoMaxSpeed)
        {
            Vector3 lVelocity = velocity.normalized * tornadoMaxSpeed;
            velocity = lVelocity;
        }
    }
}