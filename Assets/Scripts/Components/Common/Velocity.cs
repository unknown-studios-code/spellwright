using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Common
{
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }
}
