using Unity.Entities;

namespace Spellwright.Components.StatusEffect
{
    public struct StatusEffectStack : IBufferElementData
    {
        public StatusEffectType Type;
        public float Amount;
        public float RemainingDuration;
    }
}
