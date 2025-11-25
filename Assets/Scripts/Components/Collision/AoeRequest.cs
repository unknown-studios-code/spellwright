using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Collision
{
    public struct AoeRequest : IComponentData
    {
        public float3 Position;
        public float Radius;
        public Entity SourceEntity;
        public bool CheckLineOfSight;
    }
}
