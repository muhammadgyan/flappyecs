using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Svelto.ECS;
using Svelto.ECS.Hybrid;
using UnityEngine;

public class PlayerSpawnerEngine : IGetReadyEngine
{
    private GameObjectFactory _gameObjectFactory;
    private IEntityFactory _entityFactory;
    
    public PlayerSpawnerEngine(GameObjectFactory gameObjectFactory, IEntityFactory entityFactory)
    {
        _gameObjectFactory = gameObjectFactory;
        this._entityFactory = entityFactory;
    }
    
    public void Ready()
    {
        SpawnPlayer();
    }

    async void SpawnPlayer()
    {
        var playerLoadAsync =  await _gameObjectFactory.Build("Prefabs/Player");
        GameObject player = playerLoadAsync.gameObject;
        Debug.Log("Spawn Player");

        BuildPlayerEntity();
        
        void BuildPlayerEntity()
        {
            IImplementor playerRigidbodyImplementor = player.AddComponent<RigidbodyImplementor>();

            List<IImplementor> implementors = new List<IImplementor>();
            implementors.Add(playerRigidbodyImplementor);

            uint id = 1412;
            var playerEntity =_entityFactory.BuildEntity<PlayerEntityDescriptor>(id, ECSGroups.PlayersGroup, implementors);
            playerEntity.Init(new JumpForceComponent() {Value = 12});
            playerEntity.Init(new PlayerComponent(){ EntRef = new EntityReference(id)});
            
            Debug.Log("Player ID : " + playerEntity.EGID + ", Entity Reference : " + playerEntity.reference);
        }
    }
}
