using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Jobs.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.Spawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SpellSpawnSystem))]
    public partial struct SpawnRequestValidationSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();

            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<SpawnRequest>().WithNone<ValidatedTag>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var validationJob = new SpawnRequestValidationJob
            {
                ECB = ecb.AsParallelWriter(),
                SpellOwnerLookup = SystemAPI.GetComponentLookup<SpellOwner>(true),
                TransformLookup = SystemAPI.GetComponentLookup<Unity.Transforms.LocalTransform>(true),
                SpeedLookup = SystemAPI.GetComponentLookup<Speed>(true),
                LifetimeLookup = SystemAPI.GetComponentLookup<Lifetime>(true),
            };

            state.Dependency = validationJob.ScheduleParallel(_query, state.Dependency);
        }
    }
}
