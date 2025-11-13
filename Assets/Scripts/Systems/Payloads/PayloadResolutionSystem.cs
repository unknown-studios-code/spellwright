using Spellwright.Components.Collision;
using Spellwright.Components.Modifiers;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Payloads;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Systems.Payloads
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PayloadResolutionSystem : ISystem
    {
        private EntityQuery _collisionQuery;
        private ComponentLookup<PierceModifier> _pierceLookup;
        private ComponentLookup<ChainModifier> _chainLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CollisionEvent>();

            _collisionQuery = SystemAPI.QueryBuilder().WithAll<CollisionEvent, PayloadRequest>().Build();

            _pierceLookup = state.GetComponentLookup<PierceModifier>(true);
            _chainLookup = state.GetComponentLookup<ChainModifier>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            _pierceLookup.Update(ref state);
            _chainLookup.Update(ref state);

            var processJob = new ProcessPayloadsJob
            {
                ECB = ecb.AsParallelWriter(),
                PierceLookup = _pierceLookup,
                ChainLookup = _chainLookup,
            };

            state.Dependency = processJob.ScheduleParallel(_collisionQuery, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}
