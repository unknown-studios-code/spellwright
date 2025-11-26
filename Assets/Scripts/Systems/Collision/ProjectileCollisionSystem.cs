using Spellwright.Components.Collision;
using Spellwright.Jobs.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using HealthComponent = Spellwright.Components.Common.Health;

namespace Spellwright.Systems.Collision
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct ProjectileCollisionSystem : ISystem
    {
        private EntityQuery _projectileQuery;
        private ComponentLookup<ProjectileTag> _projectileLookup;
        private ComponentLookup<HealthComponent> _healthLookup;
        private ComponentLookup<LocalTransform> _transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _projectileQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ProjectileTag>().Build(ref state);
            _projectileLookup = state.GetComponentLookup<ProjectileTag>(true);
            _healthLookup = state.GetComponentLookup<HealthComponent>(true);
            _transformLookup = state.GetComponentLookup<LocalTransform>(true);

            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_projectileQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            _projectileLookup.Update(ref state);
            _healthLookup.Update(ref state);
            _transformLookup.Update(ref state);

            var job = new DetectProjectileCollisionJob
            {
                ECB = ecb.AsParallelWriter(),
                ProjectileLookup = _projectileLookup,
                HealthLookup = _healthLookup,
                TransformLookup = _transformLookup,
            };

            state.Dependency = job.Schedule(simulationSingleton, state.Dependency);
        }
    }
}
