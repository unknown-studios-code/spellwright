using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Spellwright.Components.Common;

namespace Spellwright.Tests.Utilities
{
    public static class TestHelpers
    {
        public static Entity CreateValidPrefab(EntityManager entityManager)
        {
            Entity prefab = entityManager.CreateEntity();
            entityManager.AddComponentData(prefab, new SpellOwner { OwnerEntity = Entity.Null });
            entityManager.AddComponentData(prefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            entityManager.AddComponentData(prefab, new Speed { Value = 10f });
            entityManager.AddComponentData(prefab, new Lifetime { Duration = 5f, SpawnTime = 0 });

            return prefab;
        }

        public static Entity CreateInvalidPrefab_MissingSpellOwner(EntityManager entityManager)
        {
            Entity prefab = entityManager.CreateEntity();
            entityManager.AddComponentData(prefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            entityManager.AddComponentData(prefab, new Speed { Value = 10f });
            entityManager.AddComponentData(prefab, new Lifetime { Duration = 5f, SpawnTime = 0 });

            return prefab;
        }

        public static Entity CreateInvalidPrefab_MissingLocalTransform(EntityManager entityManager)
        {
            Entity prefab = entityManager.CreateEntity();
            entityManager.AddComponentData(prefab, new SpellOwner { OwnerEntity = Entity.Null });
            entityManager.AddComponentData(prefab, new Speed { Value = 10f });
            entityManager.AddComponentData(prefab, new Lifetime { Duration = 5f, SpawnTime = 0 });

            return prefab;
        }

        public static Entity CreateInvalidPrefab_MissingSpeed(EntityManager entityManager)
        {
            Entity prefab = entityManager.CreateEntity();
            entityManager.AddComponentData(prefab, new SpellOwner { OwnerEntity = Entity.Null });
            entityManager.AddComponentData(prefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            entityManager.AddComponentData(prefab, new Lifetime { Duration = 5f, SpawnTime = 0 });

            return prefab;
        }

        public static Entity CreateInvalidPrefab_MissingLifetime(EntityManager entityManager)
        {
            Entity prefab = entityManager.CreateEntity();
            entityManager.AddComponentData(prefab, new SpellOwner { OwnerEntity = Entity.Null });
            entityManager.AddComponentData(prefab, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            entityManager.AddComponentData(prefab, new Speed { Value = 10f });

            return prefab;
        }
    }
}

