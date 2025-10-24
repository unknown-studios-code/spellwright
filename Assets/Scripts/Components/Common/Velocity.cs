using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components
{
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}
