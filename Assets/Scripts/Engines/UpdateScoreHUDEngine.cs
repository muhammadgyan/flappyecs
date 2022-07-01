using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;

public class UpdateScoreHUDEngine : IQueryingEntitiesEngine
{
    private IEntityStreamConsumerFactory _streamConsumerFactory;
    private Consumer<ScoreComponent> _consumer;
    private CancellationTokenSource _cancellationTokenSource;
    
    
    public UpdateScoreHUDEngine(IEntityStreamConsumerFactory entityStreamConsumerFactory)
    {
        _streamConsumerFactory = entityStreamConsumerFactory;
    }
    
    public void Ready()
    {
        _cancellationTokenSource = new CancellationTokenSource();
       _consumer = _streamConsumerFactory.GenerateConsumer<ScoreComponent>("Score Consumer Engine", 1);
       
       CheckScore();
    }

    async void CheckScore()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            while (_consumer.TryDequeue(out var score, out var id))
            {
                UpdateScoreHUD(score.Value);
            }

            void UpdateScoreHUD(int score)
            {
                ref var scoreHUD = ref entitiesDB.QueryEntity<HUDView>(0, ECSGroups.HUDGroup);
                scoreHUD.ScoreHUD.Score = score;
            }
            
            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }
    }
    
    public EntitiesDB entitiesDB { get; set; }
}
