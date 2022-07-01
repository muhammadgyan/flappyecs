using JetBrains.Annotations;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

// public struct PositionComponent : IEntityComponent
// {
//     public Vector3 Value;
// }

public struct PositionComponent : IEntityComponent
{
    public Vector3 Position;
}

public interface IPositionComponent
{
    Vector3 Position { get; set; }
}

public interface IRigidbody
{
    Vector3 Velocity { get; set; }
    void AddForce(float force, ForceMode2D mode);
}

public struct JumpForceComponent : IEntityComponent
{
    public float Value;
}

public struct RigidbodyViewComponent : IEntityViewComponent
{
    public IRigidbody rigidbody;
    public IColliderTriggerComponent Collide;
}

public struct PositionViewComponent : IEntityViewComponent
{
    public IPositionComponent PositionComponent;
    public EGID ID { get; set; }
}

public struct DeathComponent : IEntityComponent{}
public struct CoinComponent : IEntityComponent{}

public struct PlayerComponent : IEntityComponent
{
    public EntityReference EntRef;
}

public struct SpeedComponent : IEntityComponent
{
    public float Value;
}

public interface IColliderTriggerComponent
{
    [CanBeNull] ReactiveValue<CollisionData> colliderHit { get; set; }
}

public struct CollisionData
{
    public EnumCollisionType CollisionType
    {
        get;
        private set;
    }
    public CollisionData(EnumCollisionType collisionType)
    {
        this.CollisionType = collisionType;
    }
}

public enum EnumCollisionType
{
    Obstacle,
    Point
}

public class PlayerEntityDescriptor : IEntityDescriptor
{
    static readonly IComponentBuilder[] _componentsToBuild =
    {
        new ComponentBuilder<RigidbodyViewComponent>(),
        new ComponentBuilder<DeathComponent>(),
        new ComponentBuilder<CoinComponent>(),
        new ComponentBuilder<JumpForceComponent>(),
        new ComponentBuilder<PlayerComponent>(),
    };
        
    public IComponentBuilder[] componentsToBuild  => _componentsToBuild;

}
