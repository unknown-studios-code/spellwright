using System.Diagnostics.CodeAnalysis;
using Spellwright.Components.Health;
using Spellwright.Systems.Payloads;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using HealthComponent = Spellwright.Components.Common.Health;
using UpdateHealthJob = Spellwright.Jobs.Health.UpdateHealthJob;

namespace Spellwright.Systems.Health
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PayloadProcessingGroup))]
    public partial struct HealthUpdateSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<HealthComponent, HealthUpdateRequest>().Build(ref state);

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        [SuppressMessage("Style", "IDE0251:Make member 'readonly'")]
        public void OnUpdate(ref SystemState state)
        {
            var job = new UpdateHealthJob();

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
