using Spellwright.Components.Collision;
using Spellwright.Components.Modifiers;
using Spellwright.Components.Payloads;
using Spellwright.Components.Spawning;
using Spellwright.Components.StatusEffect;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct ProcessPayloadsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public ComponentLookup<PierceModifier> PierceLookup;

        [ReadOnly]
        public ComponentLookup<ChainModifier> ChainLookup;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity spellEntity, in DynamicBuffer<PayloadRequest> payloadBuffer, in CollisionEvent collisionEvent)
        {
            Entity target = collisionEvent.TargetEntity;

            foreach (PayloadRequest payload in payloadBuffer)
            {
                switch (payload.Type)
                {
                    case PayloadType.Damage:
                        ApplyDamage(sortKey, target, payload);
                        break;

                    case PayloadType.Heal:
                        ApplyHealing(sortKey, target, payload);
                        break;

                    case PayloadType.ApplyStatusEffect:
                        ApplyStatusEffect(sortKey, target, payload);
                        break;

                    case PayloadType.SpawnEntity:
                        SpawnEntity(sortKey, collisionEvent.ImpactPosition, payload);
                        break;

                    default:
                        break;
                }
            }

            ECB.RemoveComponent<CollisionEvent>(sortKey, spellEntity);

            bool hasPierce = PierceLookup.HasComponent(spellEntity);
            bool hasChain = ChainLookup.HasComponent(spellEntity);

            if (!hasPierce && !hasChain)
            {
                ECB.DestroyEntity(sortKey, spellEntity);
            }
        }

        [BurstCompile]
        private void ApplyDamage(int sortKey, Entity target, PayloadRequest payload)
        {
            ECB.AddBuffer<HealthModificationRequest>(sortKey, target);
            ECB.AppendToBuffer(sortKey, target, new HealthModificationRequest { Delta = -payload.Amount });
        }

        [BurstCompile]
        private void ApplyHealing(int sortKey, Entity target, PayloadRequest payload)
        {
            ECB.AddBuffer<HealthModificationRequest>(sortKey, target);
            ECB.AppendToBuffer(sortKey, target, new HealthModificationRequest { Delta = payload.Amount });
        }

        [BurstCompile]
        private void ApplyStatusEffect(int sortKey, Entity target, PayloadRequest payload)
        {
            if (payload.Effect == StatusEffectType.None)
            {
                return;
            }

            ECB.AddBuffer<StatusEffectStack>(sortKey, target);
            ECB.AppendToBuffer(
                sortKey,
                target,
                new StatusEffectStack
                {
                    Type = payload.Effect,
                    Amount = payload.Amount,
                    RemainingDuration = payload.Duration,
                }
            );
        }

        [BurstCompile]
        private void SpawnEntity(int sortKey, float3 position, PayloadRequest payload)
        {
            if (payload.SpawnPrefab == Entity.Null)
            {
                return;
            }

            Entity spawnedEntity = ECB.Instantiate(sortKey, payload.SpawnPrefab);
            ECB.AddComponent(
                sortKey,
                spawnedEntity,
                new SpawnRequest
                {
                    PrefabEntity = payload.SpawnPrefab,
                    CasterEntity = Entity.Null,
                    SpawnPosition = position,
                    SpawnDirection = default,
                }
            );
        }
    }
}
