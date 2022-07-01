
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Implementor;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using Svelto.ECS.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializerEngine : IQueryingEntitiesEngine, IDisposingEngine
{
    private IEntityFactory _entityFactory;
    private CancellationTokenSource tokenSource;
    private Consumer<InputComponent> _consumer;

    public GameInitializerEngine(IEntityFactory entityFactory, IEntityStreamConsumerFactory consumerFactory)
    {
        this._entityFactory = entityFactory;
    }
    
    public void Ready()
    {
        tokenSource = new CancellationTokenSource();
        InitGame();
        StartGame();
    }

    void InitGame()
    {
        var entity = _entityFactory.BuildEntity<GameManagerEntityDescriptor>(0, ECSGroups.GameManagerGroup);
        
        if(Debug.isDebugBuild)
            Debug.Log("Game Manager ID " + entity.EGID.entityID + " Entity Reference : " + entity.reference);
        
        entity.Init(new GameStateComponent{GameState = EnumGameState.INTRO, ID = new EGID(0, ECSGroups.GameManagerGroup)});
        entity.Init(new InputComponent{IsJump = false});
        entity.Init(new GravityMultiplierComponent{Value = 3});
        entity.Init(new MaxVelocityComponent{Value = 10});
        entity.Init(new ScoreComponent{Value = 0, ID = new EGID(0, ECSGroups.GameManagerGroup)});
        entity.Init(new InitialObstaclePosComponent{XStartPos = 12, XEndPos = -12});
    }
    
    async void StartGame()
    {
        while(!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();
        
        while(!entitiesDB.HasAny<InputComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();
        
        GameStateComponent state = entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);
        entitiesDB.PublishEntityChange<GameStateComponent>(state.ID);
        
        while (!tokenSource.IsCancellationRequested)
        {
            void UpdateState(bool isjump)
            {
                if (isjump)
                {
                    ref GameStateComponent state = ref entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);
                    if (state.GameState == EnumGameState.INTRO)
                    {
                        state.GameState = EnumGameState.PLAY;
                        entitiesDB.PublishEntityChange<GameStateComponent>(state.ID);
                    }
                    else if (state.GameState == EnumGameState.GAMEOVER)
                    {
                        SceneManager.LoadScene(0);
                    }
                }
            }
            
            var input = entitiesDB.QueryUniqueEntity<InputComponent>(ECSGroups.GameManagerGroup);
            UpdateState(input.IsJump);
           
            await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();
        }
        
    }
    
    public EntitiesDB entitiesDB { get; set; }
    public void Dispose()
    {
        tokenSource?.Cancel();
        tokenSource?.Dispose();
    }

    public bool isDisposing {
        get
        {
            return tokenSource.IsCancellationRequested;
        }
        set
        {
           
        }
    }
}
