using Spellwright.Components.Payloads;
using Spellwright.Jobs.Health;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Systems.Health
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Payloads.PayloadResolutionSystem))]
    public partial struct HealthModificationSystem : ISystem
    {
        private EntityQuery _healthModificationQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _healthModificationQuery = SystemAPI.QueryBuilder().WithAll<Components.Health, HealthModificationRequest>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_healthModificationQuery.IsEmpty)
            {
                return;
            }

            var job = new ApplyHealthModificationsJob();

            state.Dependency = job.ScheduleParallel(_healthModificationQuery, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}
