using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WorldObject : MonoBehaviour
{
    [Header("Spawn Paraeter")]
    [SerializeField] protected int m_SpawnHealtPoints = 20;
    [SerializeField] private float m_SpawnSize = 1;

    [Header("Life Paraeter")]
    [SerializeField] protected float m_HealtPoints = 20;

    [SerializeField] private Text _HP_Field;
    [SerializeField] public Slider _HPSLIDER;
    public bool isDestroyed { get; protected set; } = false;

    public virtual void Start()
    {
        _Init_Hp();
    }
    private void _Init_Hp()
    {
        m_HealtPoints = m_SpawnHealtPoints;
        UpdateUi();

        if (_HPSLIDER) _HPSLIDER.value = 0;


    }

    void UpdateUi()
    {
        if (_HP_Field) _HP_Field.text = Mathf.Ceil(m_HealtPoints).ToString();
        if (_HPSLIDER) _HPSLIDER.value = 1 - m_HealtPoints / m_SpawnHealtPoints;

    }

    public virtual void TakeDamage(float amount)
    {
         m_HealtPoints -= amount;
        math.clamp(m_HealtPoints, 0 , m_SpawnHealtPoints);
        UpdateUi();

        if (m_HealtPoints <= 0f && !isDestroyed)
        {
            Die();
        }
    }

    public virtual void TakeHealPoints(float amount)
    {
        m_HealtPoints += amount;
        UpdateUi();
    }

    protected virtual void Die()
    {
        //Destroy(gameObject);

        isDestroyed = true;
    }
}
