using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct BurnPayloadRequest : IBufferElementData
    {
        public float Amount;
        public float Duration;
    }
}
