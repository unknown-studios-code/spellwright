using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Spellwright.Components.Common;
using Spellwright.Jobs.Lifecycle;

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
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Lifetime>()
                .WithNone<Prefab>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var destroyJob = new DestroyExpiredSpellsJob
            {
                ECB = ecb.AsParallelWriter(),
                CurrentTime = SystemAPI.Time.ElapsedTime
            };

            state.Dependency = destroyJob.ScheduleParallel(_query, state.Dependency);
        }
    }
}

