using Spellwright.Components.Collision;
using Spellwright.Components.Modifiers;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Payloads;
using Spellwright.Systems.StatusEffect;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.Payloads
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(StatusEffectProcessingSystem))]
    public partial struct PayloadResolutionSystem : ISystem
    {
        private EntityQuery _query;
        private ComponentLookup<PierceModifier> _pierceLookup;
        private ComponentLookup<ChainModifier> _chainLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<CollisionEvent, PayloadRequest>().Build(ref state);
            _pierceLookup = state.GetComponentLookup<PierceModifier>(false);
            _chainLookup = state.GetComponentLookup<ChainModifier>(false);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            _pierceLookup.Update(ref state);
            _chainLookup.Update(ref state);

            var job = new ProcessPayloadsJob
            {
                ECB = ecb.AsParallelWriter(),
                PierceLookup = _pierceLookup,
                ChainLookup = _chainLookup,
            };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
