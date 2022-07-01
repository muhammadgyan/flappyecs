using System.Collections;
using System.Collections.Generic;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

public interface IDestroyableComponent
{
    void Destroy();
}

public struct DestroyableViewComponent : IEntityViewComponent
{
    public IDestroyableComponent DestroyableComponent;
}

public struct ObstacleComponent : IEntityComponent
{
    public EntityReference EntRef;
}

public class ObstacleEntityDescriptor : IEntityDescriptor
{
    static readonly IComponentBuilder[] _componentsToBuild =
    {
        new ComponentBuilder<DestroyableViewComponent>(),
        new ComponentBuilder<PositionViewComponent>(),
        new ComponentBuilder<PositionComponent>(),
        new ComponentBuilder<SpeedComponent>(),
        new ComponentBuilder<ObstacleComponent>(),
        new ComponentBuilder<DeathComponent>()
    };
        
    public IComponentBuilder[] componentsToBuild  => _componentsToBuild;
}
