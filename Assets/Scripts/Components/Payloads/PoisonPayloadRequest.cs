using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct PoisonPayloadRequest : IBufferElementData
    {
        public float Amount;
        public float Duration;
    }
}
