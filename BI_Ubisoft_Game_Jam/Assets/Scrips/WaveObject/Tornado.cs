using System.Collections;
using UnityEngine;


public class Tornado : MonoBehaviour
{
    [Header("Stats")]
    public float tornadoInitialSpeed = 10f;
    public float tornadoMaxSpeed = 10f;
    public float tornadoDamagePerSec = 10f;
    public float startLifetime = 10f;

    [Header("Behaviour Settings")]
    public bool canPassAWall = true;
    public bool randomInitialDirection = false;
    public bool spawnInWalls = false;
    public Vector3 target;

    [Header("Runtime Data")]
    public Vector3 direction;
    private Vector3 velocity;

    private float lifetime;
    private bool tryToEnterWall = true;

    [Header("Components")]
    [SerializeField] private MeshCollider _MeshCollider;

    void Awake()
    {
        // Ignore collisions between tornados
        int tornadoLayer = LayerMask.NameToLayer("Tornados");
        Physics.IgnoreLayerCollision(tornadoLayer, tornadoLayer);
    }

    void Start()
    {
        InitComponents();
        InitTarget();
        InitSpawnPosition();
        InitDirection();
        InitLifetime();
    }

    void Update()
    {
        Move();
        UpdateLifetime();
        TryEnterWallCorrection();
    }

    // ------------------------------- INIT --------------------------------

    private void InitComponents()
    {
        if (!_MeshCollider) _MeshCollider = GetComponent<MeshCollider>();
    }

    private void InitTarget()
    {
        if (target == null)
        {
            House lRandHouse = HouseManager.Instance.RandomHouse();

            if (lRandHouse) target = lRandHouse.transform.position;
            else target = Vector3.zero;
        }
    }

    private void InitSpawnPosition()
    {
        if (!spawnInWalls)
        {
            print(OutOfWallSpawnPosition.Instance.gameObject.name);
            transform.position = OutOfWallSpawnPosition.Instance.RndomPosOnCircle();
        }
        else
        {
            transform.position = SpawnerManager.Instance.ChoseRandomPositinInSpawnwers();
            canPassAWall = false;
            tryToEnterWall = false;
        }
    }

    private void InitDirection()
    {
        if (randomInitialDirection)
        {
            Vector2 rnd = Random.insideUnitCircle.normalized;
            direction = new Vector3(rnd.x, 0, rnd.y);
        }
        else
        {
            direction = (target - transform.position).normalized;
            direction.y = 0;
        }

        velocity = direction * tornadoInitialSpeed;
    }

    private void InitLifetime()
    {
        lifetime = startLifetime;
    }

    // ------------------------------- UPDATE --------------------------------

    private void Move()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void UpdateLifetime()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Destroy(gameObject);
    }

    private void TryEnterWallCorrection()
    {
        // if (tryToEnterWall)
        // {
        //     velocity += direction * 0.01f;

        //     if (velocity.magnitude > tornadoMaxSpeed)
        //         velocity = velocity.normalized * tornadoMaxSpeed;
        // }
    }

    // ---------------------------- COLLISIONS -------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (canPassAWall)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);

                StartCoroutine(ReactivateWallColision(collision));
            }
            else
            {
                Vector3 normal = collision.contacts[0].normal;
                velocity = Vector3.Reflect(velocity, normal);
                velocity.y = 0f;
            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
        House house = other.GetComponent<House>();
        if (house != null)
        {
            house.TakeDamage(tornadoDamagePerSec * Time.deltaTime);
        }
    }


    public void AddVelocity(Vector3 force)
    {
        velocity += force;

        if (velocity.magnitude > tornadoMaxSpeed)
            velocity = velocity.normalized * tornadoMaxSpeed;
    }

    private IEnumerator ReactivateWallColision(Collision pCollision)
    {
        yield return new WaitForSeconds(1);

        canPassAWall = false;
        tryToEnterWall = false;

        Physics.IgnoreCollision(pCollision.collider, _MeshCollider, false);

        print("ggoe");
    }
}