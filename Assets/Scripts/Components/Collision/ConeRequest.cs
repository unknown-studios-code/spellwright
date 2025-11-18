using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Collision
{
    public struct ConeRequest : IComponentData
    {
        public float3 Position;
        public float3 Direction;
        public float Radius;
        public float AngleDegrees;
        public Entity SourceEntity;
        public bool CheckLineOfSight;
    }
}
