using System.Diagnostics.CodeAnalysis;
using Spellwright.Components.Payloads;
using Spellwright.Jobs.Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using HealthComponent = Spellwright.Components.Health;

namespace Spellwright.Systems.Health
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Payloads.PayloadResolutionSystem))]
    public partial struct HealthModificationSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = new EntityQueryBuilder(Allocator.Temp).WithAll<HealthComponent, HealthModificationRequest>().Build(ref state);

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        [SuppressMessage("Style", "IDE0251:Make member 'readonly'")]
        public void OnUpdate(ref SystemState state)
        {
            var job = new ApplyHealthModificationsJob();

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }
}
