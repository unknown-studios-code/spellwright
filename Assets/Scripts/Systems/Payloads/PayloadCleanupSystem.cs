using Spellwright.Components.Collision;
using Spellwright.Components.Modifiers;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Systems.Payloads
{
    [BurstCompile]
    [UpdateInGroup(typeof(PayloadProcessingGroup), OrderLast = true)]
    public partial struct PayloadCleanupSystem : ISystem
    {
        private ComponentLookup<PierceModifier> _pierceLookup;
        private ComponentLookup<ChainModifier> _chainLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _pierceLookup = state.GetComponentLookup<PierceModifier>(true);
            _chainLookup = state.GetComponentLookup<ChainModifier>(true);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            _pierceLookup.Update(ref state);
            _chainLookup.Update(ref state);

            foreach ((DynamicBuffer<CollisionHit> collisions, Entity entity) in SystemAPI.Query<DynamicBuffer<CollisionHit>>().WithEntityAccess())
            {
                if (collisions.Length == 0)
                {
                    continue;
                }

                ecb.SetBuffer<CollisionHit>(entity);

                bool hasPierce = _pierceLookup.HasComponent(entity);
                bool hasChain = _chainLookup.HasComponent(entity);

                if (!hasPierce && !hasChain)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
