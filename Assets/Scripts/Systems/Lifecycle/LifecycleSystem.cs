using Spellwright.Components.Common;
using Spellwright.Jobs.Lifecycle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.Lifecycle
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LifecycleSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<Lifetime>().WithNone<Prefab>().Build(ref state);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new DestroyExpiredSpellsJob { ECB = ecb.AsParallelWriter(), CurrentTime = SystemAPI.Time.ElapsedTime };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
