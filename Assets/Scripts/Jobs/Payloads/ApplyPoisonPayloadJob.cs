using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Components.StatusEffect;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct ApplyPoisonPayloadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, in DynamicBuffer<CollisionHit> collisions, in DynamicBuffer<PoisonPayloadRequest> payloads)
        {
            foreach (CollisionHit collision in collisions)
            {
                Entity target = collision.TargetEntity;

                foreach (PoisonPayloadRequest payload in payloads)
                {
                    ECB.AppendToBuffer(
                        sortKey,
                        target,
                        new StatusEffectStack
                        {
                            Type = StatusEffectType.Poison,
                            Amount = payload.Amount,
                            RemainingDuration = payload.Duration,
                        }
                    );
                }
            }
        }
    }
}
