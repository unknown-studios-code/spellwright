using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct StunPayloadRequest : IBufferElementData
    {
        public float Duration;
    }
}
