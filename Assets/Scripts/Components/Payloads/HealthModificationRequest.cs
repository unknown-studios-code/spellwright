using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct HealthModificationRequest : IBufferElementData
    {
        public float Delta;
    }
}
