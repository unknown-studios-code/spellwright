using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Collision
{
    public struct CollisionHit : IBufferElementData
    {
        public Entity TargetEntity;
        public float3 ImpactPosition;
    }
}
