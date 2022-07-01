using Cysharp.Threading.Tasks;
using Svelto.ECS;
using UnityEngine;

public class PlayerInputEngine : IQueryingEntitiesEngine
{
    public EntitiesDB entitiesDB { get; set; }
    
    public void Ready()
    {
        ReadInput();
    }

    async void ReadInput()
    {
       
        while (true)
        {
            ReadJumpInput();
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        void ReadJumpInput()
        {
            if (!entitiesDB.HasAny<InputComponent>(ECSGroups.GameManagerGroup))
                return;
            
            ref var input = ref entitiesDB.QueryUniqueEntity<InputComponent>(ECSGroups.GameManagerGroup);
            input.IsJump = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        }
    }
}
