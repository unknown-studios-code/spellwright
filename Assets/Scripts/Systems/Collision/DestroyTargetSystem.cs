using Spellwright.Components.Collision;
using Spellwright.Jobs.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.Collision
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct DestroyTargetSystem : ISystem
    {
        private EntityQuery _collisionEventQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            _collisionEventQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<CollisionEvent>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var job = new DestroyTargetJob { ECB = ecb.AsParallelWriter() };

            state.Dependency = job.ScheduleParallel(_collisionEventQuery, state.Dependency);
        }
    }
}
