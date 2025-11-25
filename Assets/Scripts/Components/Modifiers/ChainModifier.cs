using Unity.Entities;

namespace Spellwright.Components.Modifiers
{
    public struct ChainModifier : IComponentData
    {
        public int MaxBounces;
        public int RemainingBounces;
    }
}
