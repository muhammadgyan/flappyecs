
using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

public class PlayerJumpEngine : IQueryingEntitiesEngine, IDisposingEngine
{
    private CancellationTokenSource tokenSource;
    
    public void Ready()
    {
        tokenSource = new CancellationTokenSource();
        DoJump();
    }

    async void DoJump()
    {
        while (!entitiesDB.HasAny<InputComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update);
        
        while (!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update);
        
        Debug.Log("Input Component detected");
        
        while (!tokenSource.IsCancellationRequested)
        {
            var inputEnt =  entitiesDB.QueryUniqueEntity<InputComponent>(ECSGroups.GameManagerGroup);
            var gameStateEnt =  entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);

            if (gameStateEnt.GameState != EnumGameState.GAMEOVER && inputEnt.IsJump)
            {
                var (jumps, jumpsView, total) = entitiesDB.QueryEntities<JumpForceComponent, RigidbodyViewComponent>(ECSGroups.PlayersGroup);
                for (int j = 0; j < total; j++)
                {
                    jumpsView[j].rigidbody.AddForce(jumps[j].Value, ForceMode2D.Impulse);
                }
            }
           
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
        set{}
    }
}
