using Unity.Entities;

namespace Spellwright.Components.Modifiers
{
    public struct MultiShotModifier : IComponentData
    {
        public int ProjectileCount;
        public float SpreadAngle;
    }
}
