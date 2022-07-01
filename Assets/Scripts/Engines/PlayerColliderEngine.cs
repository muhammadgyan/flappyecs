using System;
using Svelto.ECS;
using UnityEngine;

namespace Engines
{
    public class PlayerColliderEngine : IReactOnAddAndRemove<RigidbodyViewComponent>, IQueryingEntitiesEngine
    {
        public void Add(ref RigidbodyViewComponent entityComponent, EGID egid)
        {
            var player = entitiesDB.QueryEntity<PlayerComponent>(egid);
            var evt = new ReactiveValue<CollisionData>(player.EntRef, OnCollision, ReactiveType.ReactOnSet);
            entityComponent.Collide.colliderHit = evt;
        }

        public void Remove(ref RigidbodyViewComponent entityComponent, EGID egid)
        {
            entityComponent.Collide.colliderHit = null;
        }

        private void OnCollision(EntityReference entRef, CollisionData data)
        {
            if(Debug.isDebugBuild)
                Debug.Log("Collide with " + data.CollisionType);
            
            EGID id = new EGID(entRef.uniqueID, ECSGroups.PlayersGroup);

            switch (data.CollisionType)
            {
                case EnumCollisionType.Obstacle:
                    entitiesDB.PublishEntityChange<DeathComponent>(id);
                    break;
                case EnumCollisionType.Point:
                    entitiesDB.PublishEntityChange<CoinComponent>(id);
                    break;
                default:
                    break;
            }
            
        }

        public EntitiesDB entitiesDB { get; set; }
        public void Ready()
        {
            
        }
    }
}