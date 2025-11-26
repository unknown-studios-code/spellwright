using Spellwright.Components.Collision;
using Spellwright.Components.Health;
using Spellwright.Components.Payloads;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct ApplyDamagePayloadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, in DynamicBuffer<CollisionHit> collisions, in DynamicBuffer<DamagePayloadRequest> payloads)
        {
            foreach (CollisionHit collision in collisions)
            {
                Entity target = collision.TargetEntity;

                foreach (DamagePayloadRequest payload in payloads)
                {
                    ECB.AppendToBuffer(sortKey, target, new HealthUpdateRequest { Delta = -payload.Amount });
                }
            }
        }
    }
}
