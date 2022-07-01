using System.Collections;
using System.Collections.Generic;
using Svelto.ECS;
using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;
using UnityEngine;

public enum EnumGameState
{
    INTRO,
    PLAY,
    GAMEOVER
}

public struct GameStateComponent : IEntityComponent
{
    public EnumGameState GameState;
    public EGID ID;
}

public struct GameStateHUDView : IEntityViewComponent
{
    public IGameStateListener GameStateListener;
}

public interface IGameStateListener
{
    public EnumGameState State
    {
        get;
        set;
    }
}

public struct InputComponent : IEntityComponent
{
    public bool IsJump;
}

public struct GravityMultiplierComponent : IEntityComponent
{
    public float Value;
}

public struct MaxVelocityComponent : IEntityComponent
{
    public float Value;
}

public struct ScoreComponent : IEntityComponent
{
    public int Value;
    public EGID ID;
}


public struct InitialObstaclePosComponent : IEntityComponent
{
    public float XStartPos, XEndPos;
}

public class GameManagerEntityDescriptor : IEntityDescriptor
{
    static readonly IComponentBuilder[] _componentsToBuild =
    {
        new ComponentBuilder<InputComponent>(),
        new ComponentBuilder<GameStateComponent>(),
        new ComponentBuilder<GravityMultiplierComponent>(),
        new ComponentBuilder<MaxVelocityComponent>(),
        new ComponentBuilder<ScoreComponent>(),
        new ComponentBuilder<InitialObstaclePosComponent>()
    };
        
    public IComponentBuilder[] componentsToBuild  => _componentsToBuild;

}

