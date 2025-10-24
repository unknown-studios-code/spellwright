using Unity.Entities;

namespace Spellwright.Components
{
    public struct SpellOwner : IComponentData
    {
        public Entity OwnerEntity;
    }
}
