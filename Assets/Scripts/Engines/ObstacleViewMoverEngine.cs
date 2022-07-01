using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

public class ObstacleViewMoverEngine : IQueryingEntitiesEngine, IDisposingEngine
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
            var (obs, pos, view, count ) 
                = entitiesDB.QueryEntities<ObstacleComponent, PositionComponent, PositionViewComponent>(ECSGroups.ObstacleGroup);

            for (int i = 0; i < count; i++)
            {
                view[i].PositionComponent.Position = pos[i].Position;
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