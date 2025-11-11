using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;

namespace Spellwright.Tests.Utilities
{
    public static class EntityCreationUtils
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

        public static Entity CreateMovingEntity(EntityManager entityManager, float3 velocity, float3 position)
        {
            Entity entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, LocalTransform.FromPosition(position));
            entityManager.AddComponentData(entity, new Velocity { Value = velocity });
            return entity;
        }

        public static Entity CreateEntityWithVelocity(EntityManager entityManager, float3 velocity)
        {
            Entity entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, LocalTransform.FromPosition(float3.zero));
            entityManager.AddComponentData(entity, new Velocity { Value = velocity });
            return entity;
        }

        public static Entity CreateEntityWithVelocityAndRotation(EntityManager entityManager, float3 velocity, quaternion rotation)
        {
            Entity entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, LocalTransform.FromPositionRotation(float3.zero, rotation));
            entityManager.AddComponentData(entity, new Velocity { Value = velocity });
            return entity;
        }

        public static Entity CreateEntityWithVelocityAndScale(EntityManager entityManager, float3 velocity, float scale)
        {
            Entity entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, LocalTransform.FromPositionRotationScale(float3.zero, quaternion.identity, scale));
            entityManager.AddComponentData(entity, new Velocity { Value = velocity });
            return entity;
        }

        public static Entity CreateEntityWithLifetime(EntityManager entityManager, World world, double spawnTimeOffset, float duration)
        {
            Entity entity = entityManager.CreateEntity();
            double currentTime = world.Time.ElapsedTime;

            entityManager.AddComponentData(entity, LocalTransform.FromPosition(float3.zero));
            entityManager.AddComponentData(entity, new Lifetime
            {
                SpawnTime = currentTime + spawnTimeOffset,
                Duration = duration
            });

            return entity;
        }

        public static Entity CreateEntityWithLifetime(EntityManager entityManager, double spawnTime, float duration)
        {
            Entity entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new Lifetime
            {
                SpawnTime = spawnTime,
                Duration = duration
            });
            return entity;
        }

        public static Entity CreateValidatedSpawnRequest(EntityManager entityManager, Entity prefab, Entity caster, float3 position, float3 direction)
        {
            Entity request = entityManager.CreateEntity();

            entityManager.AddComponent<Prefab>(prefab);
            entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });
            entityManager.AddComponent<ValidatedTag>(request);

            return request;
        }

        public static Entity CreateSpawnRequest(EntityManager entityManager, Entity prefab, Entity caster, float3 position, float3 direction)
        {
            Entity request = entityManager.CreateEntity();
            entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });
            return request;
        }
    }
}

