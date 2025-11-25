using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Collision;
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
    public partial struct ProjectileHitSystem : ISystem
    {
        private EntityQuery _projectileQuery;
        private ComponentLookup<ProjectileTag> _projectileLookup;
        private ComponentLookup<HealthComponent> _healthLookup;
        private ComponentLookup<LocalTransform> _transformLookup;
        private BufferLookup<PayloadRequest> _payloadBufferLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _projectileQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ProjectileTag>().Build(ref state);
            _projectileLookup = state.GetComponentLookup<ProjectileTag>(true);
            _healthLookup = state.GetComponentLookup<HealthComponent>(true);
            _transformLookup = state.GetComponentLookup<LocalTransform>(true);
            _payloadBufferLookup = state.GetBufferLookup<PayloadRequest>(true);

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
            _payloadBufferLookup.Update(ref state);

            var job = new ProjectileHitJob
            {
                ECB = ecb.AsParallelWriter(),
                ProjectileLookup = _projectileLookup,
                HealthLookup = _healthLookup,
                TransformLookup = _transformLookup,
                PayloadBufferLookup = _payloadBufferLookup,
            };

            state.Dependency = job.Schedule(simulationSingleton, state.Dependency);
        }
    }
}
