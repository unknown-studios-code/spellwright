using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Spellwright.Components.Common;
using Spellwright.Jobs.Movement;

namespace Spellwright.Systems.Movement
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SpellMovementSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, Velocity>()
                .WithNone<Prefab>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var applyVelocityJob = new ApplyVelocityJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            state.Dependency = applyVelocityJob.ScheduleParallel(_query, state.Dependency);
        }
    }
}


