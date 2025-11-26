using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Components.StatusEffect;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct ApplyStunPayloadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, in DynamicBuffer<CollisionHit> collisions, in DynamicBuffer<StunPayloadRequest> payloads)
        {
            foreach (CollisionHit collision in collisions)
            {
                Entity target = collision.TargetEntity;

                foreach (StunPayloadRequest payload in payloads)
                {
                    ECB.AppendToBuffer(
                        sortKey,
                        target,
                        new StatusEffectStack
                        {
                            Type = StatusEffectType.Stun,
                            Amount = 0f,
                            RemainingDuration = payload.Duration,
                        }
                    );
                }
            }
        }
    }
}
