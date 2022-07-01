using System;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class RigidbodyImplementor : MonoBehaviour, IRigidbody, IColliderTriggerComponent, IImplementor
{
    private Rigidbody2D _rigidbody2D;
    public Vector3 Velocity {
        get
        {
            return _rigidbody2D.velocity;
        }
        set
        {
            _rigidbody2D.velocity = value;
        } 
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rigidbody2D.gravityScale = 0;
    }

    public void AddForce(float force, ForceMode2D mode)
    {
        _rigidbody2D.AddForce(Vector2.up * force, mode);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacle"))
        {
            if (colliderHit != null) colliderHit.value = new CollisionData(EnumCollisionType.Obstacle);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            if (colliderHit != null) colliderHit.value = new CollisionData(EnumCollisionType.Point);
        }
    }

    public ReactiveValue<CollisionData> colliderHit { get; set; }
}