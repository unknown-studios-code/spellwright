using Unity.Entities;

namespace Spellwright.Components.Health
{
    public struct HealthUpdateRequest : IBufferElementData
    {
        public float Delta;
    }
}
