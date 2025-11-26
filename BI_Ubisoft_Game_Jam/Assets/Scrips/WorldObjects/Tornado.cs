using System.Collections;
using UnityEngine;


public class Tornado : MonoBehaviour
{
    [Header("Stats")]
    public float tornadoInitialSpeed = 10f;
    public float tornadoMaxSpeed = 10f;
    public float tornadoDamagePerSec = 10f;
    public float startLifetime = 10f;
    [Range(0, 100)]
    public float probabilityToFocusHome = 10f;
    public float maxDistanceFromCenter = 50f;
    public float redirectSpeed = 1f;

    [Header("Behaviour Settings")]
    public bool canPassAWall = true;
    public bool randomInitialDirection = false;
    public bool spawnInWalls = false;
    public bool devienSolide = false;
    public bool folowPlayer = false;
    public bool heal = false;
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
        //CheckDistanceFromCenter();
        UpdateDirection();
    }

    // ------------------------------- INIT --------------------------------

    private void InitComponents()
    {
        if (!_MeshCollider) _MeshCollider = GetComponent<MeshCollider>();
    }

    private void InitTarget()
    {

        House lRandHouse = HouseManager.Instance.RandomHouse();
        //        print($"rand hous = {lRandHouse}");
        if (ChoosToFocusHome())
        {
            Vector2 lRandPosInGrid = new Vector2(lRandHouse.gameObject.transform.position.x, lRandHouse.gameObject.transform.position.z);
            target = new Vector3(lRandPosInGrid.x, 0, lRandPosInGrid.y);
        }
        else
        {
            target = Grid.Instance.GetRandom3DPosInFreeCells();
        }

        if (target == null)
        {
            Debug.LogWarning($"RandomHouseTargetFail");
            target = Vector3.one;
        }

    }

    private void InitSpawnPosition()
    {
        if (!spawnInWalls)
        {
            transform.position = OutOfWallSpawnPosition.Instance.RndomPosOnCircle();
        }
        else
        {
            Vector2 lRandPos = Grid.Instance.GetRandomPosInFreeCells();

            transform.position = new Vector3(lRandPos.x, 0, lRandPos.y);

            canPassAWall = false;
            tryToEnterWall = false;
        }
    }

    private void InitDirection()
    {
        if (randomInitialDirection)
        {
            Vector2 lRandPos = Random.insideUnitCircle.normalized;
            direction = new Vector3(lRandPos.x, 0, lRandPos.y);
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


    private void Move()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void UpdateLifetime()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Destroy(gameObject);
    }

    private bool ChoosToFocusHome()
    {
        float lRand = Random.Range(0, 100);

        if (lRand < probabilityToFocusHome) return true;

        return false;
    }

    // ---------------------------- COLLISIONS -------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (canPassAWall)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);

                if (devienSolide) StartCoroutine(ReactivateWallColision(collision));
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
            if (!heal)
            {
                house.TakeDamage(tornadoDamagePerSec * Time.deltaTime);
                if (!house.isDestroyed) FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.takeDamages);  //FMOD
            }
            else
            {
                house.TakeHealPoints(tornadoDamagePerSec * Time.deltaTime);  
            }
            house.HouseShake();
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
    }

    private bool CheckDistanceFromCenter()
    {
        float distance = Vector3.Distance(transform.position, Vector3.zero);

        if (distance > maxDistanceFromCenter)
        {
            return true;
        }
        return false;
    }

    private void UpdateDirection()
    {
        Vector3 desiredDirection;

        if (folowPlayer)
        {
            desiredDirection = (PlayerCanon.Instance.transform.position - transform.position).normalized;
        }
        else
        {
            if (!CheckDistanceFromCenter()) return;
            desiredDirection = (target - transform.position).normalized;
        }

        desiredDirection.y = 0f;

        direction = Vector3.RotateTowards(direction, desiredDirection, redirectSpeed * Time.deltaTime, 0f);
        direction.Normalize();
        direction.y = 0f;

        velocity = direction * velocity.magnitude;
    }
}