using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components.Spawning
{
    public struct SpawnRequest : IComponentData
    {
        public Entity PrefabEntity;
        public Entity CasterEntity;
        public float3 SpawnPosition;
        public float3 SpawnDirection;
    }
}
