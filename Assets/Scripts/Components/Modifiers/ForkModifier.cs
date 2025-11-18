using Unity.Entities;

namespace Spellwright.Components.Modifiers
{
    public struct ForkModifier : IComponentData
    {
        public int ForkCount;
        public float ForkAngle;
    }
}
