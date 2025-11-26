using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct HealPayloadRequest : IBufferElementData
    {
        public float Amount;
    }
}
