using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct SlowPayloadRequest : IBufferElementData
    {
        public float Amount;
        public float Duration;
    }
}
