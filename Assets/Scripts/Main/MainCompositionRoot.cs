using Engines;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ECS.Schedulers.Unity;

public class MainCompositionRoot : ICompositionRoot
{
    private EnginesRoot _enginesRoot;
    
    public void OnContextInitialized<T>(T contextHolder)
    {
        InitCompositionRoot();
    }

    public void OnContextDestroyed(bool hasBeenInitialised)
    {
        _enginesRoot.Dispose();
    }

    public void OnContextCreated<T>(T contextHolder)
    {
        
    }

    void InitCompositionRoot()
    {
        var unityEntitiesSubmissionScheduler = new UnityEntitiesSubmissionScheduler("FlappyECS");
        
        _enginesRoot = new EnginesRoot(unityEntitiesSubmissionScheduler);

        var entityFactory = _enginesRoot.GenerateEntityFactory();
        var entityFunction = _enginesRoot.GenerateEntityFunctions();
        var entityConsumerFactory = _enginesRoot.GenerateConsumerFactory();
        
        var gameObjectFactory = new GameObjectFactory();
        
        var gameInitializerEngine = new GameInitializerEngine(entityFactory, entityConsumerFactory);
        var hudInitializerEngine = new HUDInitializerEngine(entityFactory);
        
        var playerSpawnerEngine = new PlayerSpawnerEngine(gameObjectFactory, entityFactory);
        var playerInputEngine = new PlayerInputEngine();
        var playerGravityEngine = new PlayerGravityEngine();
        var playerColliderEngine = new PlayerColliderEngine();
        
        var obstacleSpawnerEngine = new ObstacleSpawnerEngine(gameObjectFactory, entityFactory, entityConsumerFactory);
        var obstacleMoverEngine = new ObstacleMoverEngine();
        var obstacleViewMoverEngine = new ObstacleViewMoverEngine();
        var obstacleDisposeEngine = new ObstacleDisposeEngine(entityFunction);
        
        var scoreEngine = new PlayerScoringEngine(entityConsumerFactory);
        var deadEngine = new PlayerDeadEngine(entityConsumerFactory);

        var gameStateHUDUpdaterEngine = new UpdateGameStateHUDEngine(entityConsumerFactory);
        var scoreHUDUpdaterEngine = new UpdateScoreHUDEngine(entityConsumerFactory);
        
        _enginesRoot.AddEngine(gameInitializerEngine);
        _enginesRoot.AddEngine(hudInitializerEngine);

        _enginesRoot.AddEngine(playerSpawnerEngine);
        _enginesRoot.AddEngine(playerInputEngine);
        _enginesRoot.AddEngine(playerGravityEngine);
        _enginesRoot.AddEngine(playerColliderEngine);
        
        _enginesRoot.AddEngine(obstacleSpawnerEngine);
        _enginesRoot.AddEngine(obstacleMoverEngine);
        _enginesRoot.AddEngine(obstacleViewMoverEngine);
        _enginesRoot.AddEngine(obstacleDisposeEngine);
        
        _enginesRoot.AddEngine(scoreEngine);
        _enginesRoot.AddEngine(deadEngine);
        
        _enginesRoot.AddEngine(gameStateHUDUpdaterEngine);
        _enginesRoot.AddEngine(scoreHUDUpdaterEngine);
    }
}