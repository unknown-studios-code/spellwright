using Spellwright.Components.Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using HealthComponent = Spellwright.Components.Common.Health;

namespace Spellwright.Jobs.Health
{
    [BurstCompile]
    public partial struct UpdateHealthJob : IJobEntity
    {
        [BurstCompile]
        private readonly void Execute(ref HealthComponent health, ref DynamicBuffer<HealthUpdateRequest> requests)
        {
            foreach (HealthUpdateRequest request in requests)
            {
                health.Current += request.Delta;
            }

            health.Current = math.clamp(health.Current, 0f, health.Maximum);

            requests.Clear();
        }
    }
}
