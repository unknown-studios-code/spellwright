using Unity.Entities;

namespace Spellwright.Components.Modifiers
{
    public struct PierceModifier : IComponentData
    {
        public int PierceCount;
        public int RemainingPierces;
    }
}
