using System.Threading;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

public class PlayerGravityEngine : IQueryingEntitiesEngine, IDisposingEngine
{
    private CancellationTokenSource tokenSource;

    public void Ready()
    {
        tokenSource = new CancellationTokenSource();
        DoJump();
        AddDownForce();
    }

    async void DoJump()
    {
        while (!entitiesDB.HasAny<InputComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();

        while (!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
            await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();
    
        while (!tokenSource.IsCancellationRequested)
        {
            var inputEnt = entitiesDB.QueryUniqueEntity<InputComponent>(ECSGroups.GameManagerGroup);
            var gameStateEnt = entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);
           
            if (gameStateEnt.GameState == EnumGameState.PLAY && inputEnt.IsJump)
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
    
    async void AddDownForce()
    {
        while (!tokenSource.IsCancellationRequested)
        {
            while (!entitiesDB.HasAny<GameStateComponent>(ECSGroups.GameManagerGroup))
                await UniTask.Yield(PlayerLoopTiming.Update, tokenSource.Token).SuppressCancellationThrow();
            
            var gameStateEnt = entitiesDB.QueryUniqueEntity<GameStateComponent>(ECSGroups.GameManagerGroup);

            if (gameStateEnt.GameState == EnumGameState.PLAY)
            {
                var gravity = entitiesDB.QueryUniqueEntity<GravityMultiplierComponent>(ECSGroups.GameManagerGroup);
                var velocities = entitiesDB.QueryUniqueEntity<MaxVelocityComponent>(ECSGroups.GameManagerGroup);
                
                var (players, forces, view, total) = entitiesDB.QueryEntities<PlayerComponent, JumpForceComponent, RigidbodyViewComponent>(ECSGroups
                    .PlayersGroup);

                for (int j = 0; j < total; j++)
                {
                    view[j].rigidbody.AddForce(Physics.gravity.y * gravity.Value, ForceMode2D.Force);
                    view[j].rigidbody.Velocity =
                        Vector3.ClampMagnitude(view[j].rigidbody.Velocity, velocities.Value);
                }
            }
            
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, tokenSource.Token).SuppressCancellationThrow();
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
        set
        {
           
        }
    }
}
