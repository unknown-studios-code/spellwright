using Unity.Entities;

namespace Spellwright.Components.Common
{
    public struct SpellOwner : IComponentData
    {
        public Entity OwnerEntity;
    }
}
