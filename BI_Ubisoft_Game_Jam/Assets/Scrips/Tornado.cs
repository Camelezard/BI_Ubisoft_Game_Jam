using System.Runtime.CompilerServices;
using UnityEngine;


public class Tornado : MonoBehaviour
{
    [SerializeField] private float _TornadoSpeed = 10;
    [SerializeField] private float _TornadoDamamges = 10;
    [SerializeField] private float _TornadoWeight = 10;
    private Vector3 _Direction;
    void Start()
    {
        ChooseInitialDirection();
    }

    private void Update()
    {
        transform.position += _Direction * _TornadoSpeed * Time.deltaTime;
    }

    private void ChooseInitialDirection()
    {
        Vector2 _Circle = Random.insideUnitCircle.normalized;
        _Direction = new Vector3(_Circle.x, 0, _Circle.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;

        _Direction = Vector3.Reflect(_Direction, normal);

        _Direction.y = 0f;

        print("OnCollisionEnter");
    }

    
}
