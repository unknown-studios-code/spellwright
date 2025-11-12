using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Collision
{
    public struct CollisionEvent : IComponentData
    {
        public Entity ProjectileEntity;
        public Entity TargetEntity;
        public float3 ImpactPosition;
    }
}
