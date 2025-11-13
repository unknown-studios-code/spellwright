using Unity.Entities;

namespace Spellwright.Components
{
    public struct Health : IComponentData
    {
        public float Current;
        public float Maximum;
    }
}
