using Spellwright.Components.Payloads;
using Spellwright.Components.StatusEffect;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.StatusEffect
{
    [BurstCompile]
    public partial struct ProcessStatusEffectsJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref DynamicBuffer<StatusEffectStack> effects)
        {
            for (int i = effects.Length - 1; i >= 0; i--)
            {
                StatusEffectStack effect = effects[i];

                switch (effect.Type)
                {
                    case StatusEffectType.Burn:
                    case StatusEffectType.Poison:
                        ApplyDamageOverTime(sortKey, entity, effect.Amount);
                        break;

                    case StatusEffectType.Slow:
                    case StatusEffectType.Stun:
                    case StatusEffectType.None:
                    default:
                        break;
                }

                effect.RemainingDuration -= DeltaTime;

                if (effect.RemainingDuration <= 0f)
                {
                    effects.RemoveAt(i);
                }
                else
                {
                    effects[i] = effect;
                }
            }
        }

        [BurstCompile]
        private void ApplyDamageOverTime(int sortKey, Entity entity, float damagePerSecond)
        {
            float damage = damagePerSecond * DeltaTime;

            ECB.AddBuffer<HealthModificationRequest>(sortKey, entity);
            ECB.AppendToBuffer(sortKey, entity, new HealthModificationRequest { Delta = -damage });
        }
    }
}
