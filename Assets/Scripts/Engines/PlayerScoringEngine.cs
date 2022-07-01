using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

namespace Engines
{
    public class PlayerScoringEngine : IQueryingEntitiesEngine, IDisposingEngine
    {
        private readonly IEntityStreamConsumerFactory _entityStreamConsumerFactory;
        private Consumer<CoinComponent> _consumer;
        private CancellationTokenSource _cancellationTokenSource;
        
        public PlayerScoringEngine(IEntityStreamConsumerFactory entityStreamConsumerFactory)
        {
            this._entityStreamConsumerFactory = entityStreamConsumerFactory;
        }
        
        public void Ready()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumer = _entityStreamConsumerFactory.GenerateConsumer<CoinComponent>("CoinEngine", 1);
            GetPoin();
        }

        async void GetPoin()
        {
            bool cancelled;

            while (true)
            {
                while (_consumer.TryDequeue(out var ent, out var id))
                {
                    cancelled = await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();

                    if (cancelled)
                        return;
                    
                    AddScore();

                    if (Debug.isDebugBuild)
                    {
                        var ScoreEnt = entitiesDB.QueryEntity<ScoreComponent>(0, ECSGroups.GameManagerGroup);
                        Debug.Log(ScoreEnt.Value);
                    }
                }

                cancelled = await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
                
                if (cancelled)
                    return;
            }

            void AddScore()
            {
                ref var scoreEnt = ref entitiesDB.QueryEntity<ScoreComponent>(0, ECSGroups.GameManagerGroup);
                scoreEnt.Value += 1;

                ref var scoreHUD = ref entitiesDB.QueryEntity<HUDView>(0, ECSGroups.HUDGroup);
                scoreHUD.ScoreHUD.Score = scoreEnt.Value;

                _consumer.Flush();
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
        } set { } 
        }
    }
}