using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Implementor;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class ObstacleSpawnerEngine : IQueryingEntitiesEngine, IDisposingEngine
{
    private GameObjectFactory gameObjectFactory;
    private IEntityFactory _entityFactory;
    private IEntityStreamConsumerFactory _entityStreamConsumerFactory;
    Consumer<GameStateComponent>          _consumer;

    private bool isSpawningEnemies = false;
    private CancellationTokenSource _tokenSource;
    
    private uint enemiesCreated = 0;

    public ObstacleSpawnerEngine(GameObjectFactory gameObjectFactory, IEntityFactory entityFactory, IEntityStreamConsumerFactory entityStreamConsumerFactory)
    {
        this.gameObjectFactory = gameObjectFactory;
        this._entityFactory = entityFactory;
        this._entityStreamConsumerFactory = entityStreamConsumerFactory;
        _tokenSource = new CancellationTokenSource();
    }
    
    public void Ready()
    {
        _consumer = _entityStreamConsumerFactory.GenerateConsumer<GameStateComponent>("GameStateEngine", 1);
        CheckGameState();
        SpawnObstacle();
    }

    async void CheckGameState()
    {
        while (!isSpawningEnemies && !_tokenSource.IsCancellationRequested)
        {
            var isCancelled = await UniTask.Yield(PlayerLoopTiming.Update, _tokenSource.Token).SuppressCancellationThrow();

            if (isCancelled)
                return;
            
            while(_consumer.TryDequeue(out var e, out var id))
            {
                isSpawningEnemies = e.GameState == EnumGameState.PLAY;
            }
        }
    }

    async void SpawnObstacle()
    {
        while (!_tokenSource.IsCancellationRequested)
        {
            var isCancelled = await UniTask.Yield(PlayerLoopTiming.Update, _tokenSource.Token).SuppressCancellationThrow();
            if (isCancelled)
                return;

            while (!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
                await UniTask.Yield(PlayerLoopTiming.Update, _tokenSource.Token).SuppressCancellationThrow();
                
            while (!entitiesDB.HasAny<InitialObstaclePosComponent>(ECSGroups.GameManagerGroup))
                await UniTask.Yield(PlayerLoopTiming.Update, _tokenSource.Token).SuppressCancellationThrow();
            
            var gameStateEnt = entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);
            
            if (isSpawningEnemies && gameStateEnt.GameState == EnumGameState.PLAY)
            {
                isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(3), 
                    DelayType.Realtime, 
                    PlayerLoopTiming.Update, 
                    _tokenSource.Token).SuppressCancellationThrow();

                if (isCancelled)
                    return;

                var initialObstaclePosEnt = entitiesDB.QueryUniqueEntity<InitialObstaclePosComponent>(ECSGroups.GameManagerGroup);
                var loadObstacle = await gameObjectFactory.Build("Prefabs/Obstacle");
                GameObject obstacle = loadObstacle.gameObject;
                
                Vector3 curPos = obstacle.transform.position;
                curPos.x = initialObstaclePosEnt.XStartPos;
                curPos.y = UnityEngine.Random.Range(-3f, 3f);
                obstacle.transform.position = curPos;
                
                List<IImplementor> implementors = new List<IImplementor>();
                implementors.Add(obstacle.AddComponent<PositionImplementor>());
                implementors.Add(obstacle.AddComponent<DestroyableImplementor>());

                BuildObstacleEntity(implementors, obstacle);
            }
        }

        void BuildObstacleEntity(List<IImplementor> implementors, GameObject go)
        {
            var obstacle =
                _entityFactory.BuildEntity<ObstacleEntityDescriptor>(enemiesCreated++, ECSGroups.ObstacleGroup,
                    implementors);
            
            obstacle.Init(new ObstacleComponent(){EntRef = new EntityReference(obstacle.EGID.entityID)});
            obstacle.Init(new SpeedComponent() {Value = 4});
            obstacle.Init(new PositionComponent() {Position = go.transform.position});
        }
    }
    public EntitiesDB entitiesDB { get; set; }
    public void Dispose()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

    public bool isDisposing {
        get
        {
            return _tokenSource.IsCancellationRequested;
        }
        set{}
    }
}
