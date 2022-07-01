using System.Collections;
using System.Collections.Generic;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class HUDInitializerEngine : IGetReadyEngine
{
    private IEntityFactory _entityFactory;
    
    public HUDInitializerEngine(IEntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public void Ready()
    {
        InitHUD();
    }

    void InitHUD()
    {
        List<IImplementor> implementors = new List<IImplementor>();
        implementors.Add(Object.FindObjectOfType<GameStateHUDImplementor>());
        implementors.Add(Object.FindObjectOfType<ScoreHUDImplementor>());

        _entityFactory.BuildEntity<HUDEntityDescriptor>(0, ECSGroups.HUDGroup, implementors);
    }
}
