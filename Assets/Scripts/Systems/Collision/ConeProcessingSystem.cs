using Spellwright.Components.Collision;
using Spellwright.Jobs.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Spellwright.Systems.Collision
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(AoeProcessingSystem))]
    public partial struct ConeProcessingSystem : ISystem
    {
        private EntityQuery _coneRequestQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _coneRequestQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ConeRequest>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ConeProcessingJob
            {
                ECB = ecb.AsParallelWriter(),
                CollisionWorld = physicsWorld.CollisionWorld,
                DamageableLookup = SystemAPI.GetComponentLookup<DamageableTag>(true),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
            };

            state.Dependency = job.ScheduleParallel(_coneRequestQuery, state.Dependency);
        }
    }
}
