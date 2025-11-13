using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct PayloadElement : IBufferElementData
    {
        public PayloadType Type;
        public ElementalType Element;
        public StatusEffectType Effect;
        public float Amount;
        public float Duration;
    }
}
