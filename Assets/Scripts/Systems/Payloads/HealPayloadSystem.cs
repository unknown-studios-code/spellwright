using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Payloads;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.Payloads
{
    [BurstCompile]
    [UpdateInGroup(typeof(PayloadProcessingGroup))]
    public partial struct HealPayloadSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<CollisionHit, HealPayloadRequest>().Build(ref state);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ApplyHealPayloadJob { ECB = ecb.AsParallelWriter() };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
