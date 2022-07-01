
using System;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class PositionImplementor : MonoBehaviour, IPositionComponent, IImplementor
{
    private Transform _transform;
    
    public Vector3 Position {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }
}
