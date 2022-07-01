using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

namespace Engines
{
    public class PlayerDeadEngine : IQueryingEntitiesEngine, IDisposingEngine
    {
        private readonly IEntityStreamConsumerFactory _entityStreamConsumerFactory;
        private Consumer<DeathComponent> _consumer;
        private CancellationTokenSource _cancellationTokenSource;
        
        public PlayerDeadEngine(IEntityStreamConsumerFactory entityStreamConsumerFactory)
        {
            this._entityStreamConsumerFactory = entityStreamConsumerFactory;
        }
        
        public void Ready()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumer = _entityStreamConsumerFactory.GenerateConsumer<DeathComponent>("DeadEngine", 1);
            GetPoin();
        }

        async void GetPoin()
        {
            bool isCancelled = false;
            
            while (true)
            {
                isCancelled = await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
                
                if(isCancelled)
                    return;
                
                while (_consumer.TryDequeue(out var ent, out var id))
                {
                    SetGameOver();

                    if (Debug.isDebugBuild)
                    {
                        var gameStateEnt = entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);
                        Debug.Log(gameStateEnt.GameState);
                    }
                   
                }
            }

            void SetGameOver()
            {
                ref var gameStateEnt = ref entitiesDB.QueryEntity<GameStateComponent>(0, ECSGroups.GameManagerGroup);
                gameStateEnt.GameState = EnumGameState.GAMEOVER;
                
                entitiesDB.PublishEntityChange<GameStateComponent>(new EGID(gameStateEnt.EntRef.uniqueID, ECSGroups.GameManagerGroup));

                ResetVelocity();
                
                _consumer.Flush();
            }
        }

        void ResetVelocity()
        {
            var(player, view, count) =  entitiesDB.QueryEntities<PlayerComponent, RigidbodyViewComponent>(ECSGroups.PlayersGroup);
            for (int i = 0; i < count; i++)
            {
                view[i].rigidbody.Velocity = Vector3.zero;
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
}