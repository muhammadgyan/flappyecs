using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;

namespace Engines
{
    public class ObstacleDisposeEngine : IQueryingEntitiesEngine, IDisposingEngine
    {
        private IEntityFunctions _entityFunctions;
        private CancellationTokenSource _cancellationTokenSource;
        
        public ObstacleDisposeEngine(IEntityFunctions entityFunctions)
        {
            this._entityFunctions = entityFunctions;
        }
        
        public void Ready()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CheckObstaclePos();
        }


        async void CheckObstaclePos()
        {
            while (!entitiesDB.HasAny<InitialObstaclePosComponent>(ECSGroups.GameManagerGroup))
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var InitialObsPosEnt =
                    entitiesDB.QueryUniqueEntity<InitialObstaclePosComponent>(ECSGroups.GameManagerGroup);
                var (obs, pos, destroyables, count) = entitiesDB.QueryEntities<ObstacleComponent, PositionComponent, DestroyableViewComponent>(ECSGroups.ObstacleGroup);

                for (int i = 0; i < count; i++)
                {
                    if (pos[i].Position.x <= InitialObsPosEnt.XEndPos)
                    {
                        _entityFunctions.RemoveEntity<ObstacleEntityDescriptor>(new EGID(obs[i].EntRef.uniqueID, ECSGroups.ObstacleGroup)); 
                        destroyables[i].DestroyableComponent.Destroy();
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

        public bool isDisposing {
            get
            {
                return _cancellationTokenSource.IsCancellationRequested;
            }
            set{} }
    }
}