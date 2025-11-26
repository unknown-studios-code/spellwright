using Unity.Entities;

namespace Spellwright.Components.Payloads
{
    public struct SpawnEntityRequest : IBufferElementData
    {
        public Entity Prefab;
    }
}
