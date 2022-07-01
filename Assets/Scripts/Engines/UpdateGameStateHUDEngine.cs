using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;

public class UpdateGameStateHUDEngine : IQueryingEntitiesEngine
{
    private IEntityStreamConsumerFactory _entityStreamConsumerFactory;
    private CancellationTokenSource _cancellationTokenSource;
    private Consumer<GameStateComponent> _consumer;

    public UpdateGameStateHUDEngine(IEntityStreamConsumerFactory entityStreamConsumerFactory)
    {
        _entityStreamConsumerFactory = entityStreamConsumerFactory;
    }
    
    public void Ready()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _consumer = _entityStreamConsumerFactory.GenerateConsumer<GameStateComponent>("Game State Listener Engine", 1);
        CheckGameState();
    }

    async void CheckGameState()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            while (_consumer.TryDequeue(out var state, out var id))
            {
                SetGameStateListener(state.GameState);
            }

            void SetGameStateListener(EnumGameState state)
            {
                ref var entity = ref entitiesDB.QueryEntity<GameStateHUDView>(0, ECSGroups.HUDGroup);
                entity.GameStateListener.State = state;
            }
            
            await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }
    }

   
    
    public EntitiesDB entitiesDB { get; set; }
}
