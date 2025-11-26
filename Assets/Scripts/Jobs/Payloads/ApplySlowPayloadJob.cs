using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Components.StatusEffect;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct ApplySlowPayloadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, in DynamicBuffer<CollisionHit> collisions, in DynamicBuffer<SlowPayloadRequest> payloads)
        {
            foreach (CollisionHit collision in collisions)
            {
                Entity target = collision.TargetEntity;

                foreach (SlowPayloadRequest payload in payloads)
                {
                    ECB.AppendToBuffer(
                        sortKey,
                        target,
                        new StatusEffectStack
                        {
                            Type = StatusEffectType.Slow,
                            Amount = payload.Amount,
                            RemainingDuration = payload.Duration,
                        }
                    );
                }
            }
        }
    }
}
