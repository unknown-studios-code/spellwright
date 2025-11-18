using Spellwright.Components.Collision;
using Spellwright.Jobs.Collision;
using Unity.Burst;
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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ProjectileHitJob
            {
                ECB = ecb.AsParallelWriter(),
                ProjectileLookup = SystemAPI.GetComponentLookup<ProjectileTag>(true),
                HealthLookup = SystemAPI.GetComponentLookup<HealthComponent>(true),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            };

            state.Dependency = job.Schedule(simulationSingleton, state.Dependency);
        }
    }
}
