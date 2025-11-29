using System.Collections;
using System.Linq;
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
    public bool obstructView = false;
    public Vector3 target;
    public float playerDetectionRadius = 2f;
    public float apearDuration = 4f;
    private bool isapparing = true;

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
        //FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.torandoAppear);

        InitComponents();
        InitTarget();
        InitSpawnPosition();

        InitLifetime();
        InitApparition();
    }

    void Update()
    {
        Move();
        UpdateLifetime();
        //CheckDistanceFromCenter();
        UpdateDirection();

        if (obstructView) checkDistAtPlayer();
    }

    void checkDistAtPlayer()
    {
        float sqrDistance = Vector3.Distance(PlayerCanon.Instance.transform.position, transform.position);

        //print(sqrDistance);
        if (sqrDistance < playerDetectionRadius)
        {
            //print("close" + sqrDistance);

            if (!playerInside)
                OnPlayerClose();
        }
        else
        {
            //print("far" + sqrDistance);

            if (playerInside)
                OnPlayerFar();
        }
    }

    // ------------------------------- INIT --------------------------------

    private void InitComponents()
    {
        if (!_MeshCollider) _MeshCollider = GetComponent<MeshCollider>();
    }

    private void InitTarget()
    {
        // Défauts
        Vector3 fallbackTarget = Vector3.one * 10f;

        // Vérification HouseManager
        if (HouseManager.Instance == null)
        {
            Debug.LogWarning("InitTarget: HouseManager.Instance is NULL. Using fallback target.");
            target = fallbackTarget;
            return;
        }

        House lRandHouse = HouseManager.Instance.RandomHouse();

        if (ChoosToFocusHome() && lRandHouse != null)
        {
            Vector3 housePos = lRandHouse.gameObject.transform.position;
            target = new Vector3(housePos.x, 0f, housePos.z);
        }
        else
        {
            // Try Grid first
            if (Grid.Instance != null)
            {
                try
                {
                    target = Grid.Instance.GetRandom3DPosInFreeCells();
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"InitTarget: Grid.Instance.GetRandom3DPosInFreeCells() failed: {ex.Message}");
                    target = fallbackTarget;
                }
            }
            else
            {
                Debug.LogWarning("InitTarget: Grid.Instance is NULL. Using fallback target.");
                target = fallbackTarget;
            }
        }

        // Extra safety: never leave target uninitialized
        if (target == Vector3.zero)
        {
            Debug.LogWarning("InitTarget: computed target is Vector3.zero, using fallback.");
            target = fallbackTarget;
        }
    }

    private void InitSpawnPosition()
    {
        // Vérification OutOfWallSpawnPosition
        if (!spawnInWalls)
        {
            if (OutOfWallSpawnPosition.Instance != null)
            {
                transform.position = OutOfWallSpawnPosition.Instance.RndomPosOnCircle();
            }
            else
            {
                Debug.LogWarning("InitSpawnPosition: OutOfWallSpawnPosition.Instance is NULL. Using Grid or fallback.");
                // fallback to Grid
                if (Grid.Instance != null)
                {
                    Vector2 pos = Grid.Instance.GetRandomPosInFreeCells();
                    transform.position = new Vector3(pos.x, 0f, pos.y);
                }
                else
                {
                    Debug.LogWarning("InitSpawnPosition: Grid.Instance is also NULL. Using origin.");
                    transform.position = Vector3.zero;
                }
            }
        }
        else
        {
            if (Grid.Instance != null)
            {
                Vector2 lRandPos = Grid.Instance.GetRandomPosInFreeCells();
                transform.position = new Vector3(lRandPos.x, 0, lRandPos.y);

                canPassAWall = false;
                tryToEnterWall = false;
            }
            else
            {
                Debug.LogWarning("InitSpawnPosition (spawnInWalls): Grid.Instance is NULL. Using origin.");
                transform.position = Vector3.zero;
                canPassAWall = false;
                tryToEnterWall = false;
            }
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

    private void InitApparition()
    {
        StartCoroutine(apparition());
    }

    private IEnumerator apparition()
    {
        isapparing = true;
        float elapsedTime = 0;
        float ratio = 0;

        Transform[] visualObjects = gameObject.GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();

        while (elapsedTime <= apearDuration)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / apearDuration;

            foreach (Transform child in visualObjects) child.transform.localScale = Vector3.one * ratio;
            yield return null;
        }

        InitDirection();

        isapparing = false;

        yield return null;
    }

    private void Move()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void UpdateLifetime()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            int count = TornadoWaveManager.instance.childCount--;

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ambienceIntensity", count);

            Destroy(gameObject);
            if (obstructView) UiManager.Instance.TornadoDestroy();
            //FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.torandoDisappear);

        }
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
        if (house != null && !isapparing)
        {
            if (!heal)
            {
                house.TakeDamage(tornadoDamagePerSec * Time.deltaTime);
                if (!house.isDestroyed) FMODUnity.RuntimeManager.PlayOneShot(SoundManager.Instance.houseTakeDamages);  //FMOD
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
        if (!isapparing) velocity += force;

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



    private Coroutine fadeCoroutine;
    private bool playerInside = false;



    private void OnPlayerClose()
    {
        playerInside = true;
        UiManager.Instance.StartFadeInObstruction();
    }

    private void OnPlayerFar()
    {
        playerInside = false;
        UiManager.Instance.StartFadeOutObstruction();
    }

}