using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Jobs.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Spellwright.Systems.Spawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpellSpawnSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<SpawnRequest, ValidatedTag>().Build(ref state);

            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var spawnJob = new SpawnSpellJob
            {
                ECB = ecb.AsParallelWriter(),
                SpellOwnerLookup = SystemAPI.GetComponentLookup<SpellOwner>(true),
                TransformLookup = SystemAPI.GetComponentLookup<Unity.Transforms.LocalTransform>(true),
                SpeedLookup = SystemAPI.GetComponentLookup<Speed>(true),
                LifetimeLookup = SystemAPI.GetComponentLookup<Lifetime>(true),
                PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(true),
                CurrentTime = SystemAPI.Time.ElapsedTime,
            };

            state.Dependency = spawnJob.ScheduleParallel(_query, state.Dependency);
        }
    }
}
