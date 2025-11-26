using Unity.Entities;

namespace Spellwright.Components.Common
{
    public struct Health : IComponentData
    {
        public float Current;
        public float Maximum;
    }
}
