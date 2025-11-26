using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct DamagePayloadRequest : IBufferElementData
    {
        public ElementalType Element;
        public float Amount;
    }
}
