using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Collision;
using Spellwright.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using HealthComponent = Spellwright.Components.Health;

namespace Spellwright.Systems.Collision
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct AoeProcessingSystem : ISystem
    {
        private EntityQuery _query;
        private ComponentLookup<HealthComponent> _healthLookup;
        private ComponentLookup<LocalTransform> _transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<AoeRequest, PayloadRequest>().Build(ref state);
            _healthLookup = state.GetComponentLookup<HealthComponent>(true);
            _transformLookup = state.GetComponentLookup<LocalTransform>(true);

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            _healthLookup.Update(ref state);
            _transformLookup.Update(ref state);

            var job = new AoeProcessingJob
            {
                ECB = ecb.AsParallelWriter(),
                CollisionWorld = physicsWorld.CollisionWorld,
                HealthLookup = _healthLookup,
                TransformLookup = _transformLookup,
                EnvironmentFilter = CollisionUtils.CreateEnvironmentFilter(),
            };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
