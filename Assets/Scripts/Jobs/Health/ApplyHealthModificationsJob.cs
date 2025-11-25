using Spellwright.Components.Payloads;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Jobs.Health
{
    [BurstCompile]
    public partial struct ApplyHealthModificationsJob : IJobEntity
    {
        [BurstCompile]
        private readonly void Execute(ref Components.Health health, ref DynamicBuffer<HealthModificationRequest> modifierBuffer)
        {
            foreach (HealthModificationRequest modifier in modifierBuffer)
            {
                health.Current += modifier.Delta;
            }

            health.Current = math.clamp(health.Current, 0f, health.Maximum);

            modifierBuffer.Clear();
        }
    }
}
