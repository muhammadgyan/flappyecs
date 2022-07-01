using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

public class ObstacleMoverEngine : IQueryingEntitiesEngine, IDisposingEngine
{
    private CancellationTokenSource _cancellationTokenSource;
    
    public void Ready()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Move();
    }

    async void Move()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            while (!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
            
            var gameStateEnt =  entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);

            if (gameStateEnt.GameState == EnumGameState.PLAY)
            {
                var (obs, spd, pos, count ) 
                    = entitiesDB.QueryEntities<ObstacleComponent, SpeedComponent, PositionComponent>(ECSGroups.ObstacleGroup);

                for (int i = 0; i < count; i++)
                {
                    Vector2 obstaclePos = pos[i].Position;
                    obstaclePos.x += -1 * spd[i].Value * Time.deltaTime;
                    pos[i].Position = obstaclePos;
                }

            }
            
            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }    
    }
    
    public EntitiesDB entitiesDB { get; set; }
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    public bool isDisposing { get
    {
        return _cancellationTokenSource.IsCancellationRequested;
    } set {} }
}
