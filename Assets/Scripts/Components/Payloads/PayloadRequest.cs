using Spellwright.Components.StatusEffect;
using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct PayloadRequest : IBufferElementData
    {
        public PayloadType Type;
        public ElementalType Element;
        public StatusEffectType Effect;
        public Entity SpawnPrefab;
        public float Amount;
        public float Duration;
    }
}
