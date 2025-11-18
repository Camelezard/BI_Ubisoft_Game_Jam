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

    public virtual void Start()
    {
        _Init_Hp();
    }
    private void _Init_Hp()
    {
        m_HealtPoints = m_SpawnHealtPoints;
        UpdateUi();
    }

    void UpdateUi()
    {
        _HP_Field.text = m_HealtPoints.ToString();
    }

    public virtual void TakeDamage(float amount)
    {
        m_HealtPoints -= amount;
        UpdateUi();

        if (m_HealtPoints <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
