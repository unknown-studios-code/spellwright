using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Spellwright.Components.Common;

namespace Spellwright.Tests.EditMode.Systems.Lifecycle
{
    [TestFixture]
    public class LifecycleSystemTests
    {
        private World _world;
        private EntityManager _entityManager;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
        }

        [TearDown]
        public void TearDown()
        {
            if (_world != null && _world.IsCreated)
            {
                _world.Dispose();
            }
        }

        [Test]
        public void Query_WithLifetime_ReturnsEntity()
        {
            Entity entity = CreateEntityWithLifetime(0.0, 5.0f);

            var query = _entityManager.CreateEntityQuery(typeof(Lifetime));
            int count = query.CalculateEntityCount();

            Assert.AreEqual(1, count, "Should find entity with Lifetime component");
        }

        [Test]
        public void Query_WithoutLifetime_ReturnsZero()
        {
            _entityManager.CreateEntity();

            var query = _entityManager.CreateEntityQuery(typeof(Lifetime));
            int count = query.CalculateEntityCount();

            Assert.AreEqual(0, count, "Should not find entities without Lifetime component");
        }

        [Test]
        public void Query_WithPrefabTag_IsExcluded()
        {
            Entity prefab = CreateEntityWithLifetime(0.0, 5.0f);
            _entityManager.AddComponent<Prefab>(prefab);

            var query = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Lifetime>(),
                ComponentType.Exclude<Prefab>()
            );
            int count = query.CalculateEntityCount();

            Assert.AreEqual(0, count, "Prefabs should be excluded from query");
        }

        [Test]
        public void Query_WithoutPrefabTag_IsIncluded()
        {
            Entity entity = CreateEntityWithLifetime(0.0, 5.0f);

            var query = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Lifetime>(),
                ComponentType.Exclude<Prefab>()
            );
            int count = query.CalculateEntityCount();

            Assert.AreEqual(1, count, "Non-prefab entities should be included");
        }

        [Test]
        public void Query_MixedPrefabsAndEntities_OnlyReturnsNonPrefabs()
        {
            Entity prefab1 = CreateEntityWithLifetime(0.0, 5.0f);
            _entityManager.AddComponent<Prefab>(prefab1);

            Entity prefab2 = CreateEntityWithLifetime(1.0, 3.0f);
            _entityManager.AddComponent<Prefab>(prefab2);

            Entity entity1 = CreateEntityWithLifetime(2.0, 7.0f);
            Entity entity2 = CreateEntityWithLifetime(3.0, 2.0f);

            var query = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Lifetime>(),
                ComponentType.Exclude<Prefab>()
            );
            int count = query.CalculateEntityCount();

            Assert.AreEqual(2, count, "Should only return non-prefab entities");
        }

        [Test]
        public void Query_ReturnsCorrectEntities()
        {
            Entity entity1 = CreateEntityWithLifetime(0.0, 5.0f);
            Entity entity2 = CreateEntityWithLifetime(1.0, 3.0f);
            Entity entity3 = CreateEntityWithLifetime(2.0, 7.0f);

            var query = _entityManager.CreateEntityQuery(typeof(Lifetime));
            var entities = query.ToEntityArray(Allocator.Temp);

            Assert.AreEqual(3, entities.Length, "Should return all three entities");
            entities.Dispose();
        }

        [Test]
        public void Lifetime_Component_HasCorrectData()
        {
            double spawnTime = 10.5;
            float duration = 7.5f;
            Entity entity = CreateEntityWithLifetime(spawnTime, duration);

            Lifetime lifetime = _entityManager.GetComponentData<Lifetime>(entity);

            Assert.AreEqual(spawnTime, lifetime.SpawnTime, "SpawnTime should match");
            Assert.AreEqual(duration, lifetime.Duration, "Duration should match");
        }

        [Test]
        public void Entity_CanBeDestroyedAfterExpiration()
        {
            Entity entity = CreateEntityWithLifetime(0.0, 5.0f);

            Assert.IsTrue(_entityManager.Exists(entity), "Entity should exist before destruction");

            _entityManager.DestroyEntity(entity);

            Assert.IsFalse(_entityManager.Exists(entity), "Entity should not exist after destruction");
        }

        [Test]
        public void Query_AfterEntityDestroyed_ReturnsZero()
        {
            Entity entity = CreateEntityWithLifetime(0.0, 5.0f);
            _entityManager.DestroyEntity(entity);

            var query = _entityManager.CreateEntityQuery(typeof(Lifetime));
            int count = query.CalculateEntityCount();

            Assert.AreEqual(0, count, "Query should return zero after entity destroyed");
        }

        [Test]
        public void PrefabTag_DoesNotAffectLifetimeComponent()
        {
            Entity prefab = CreateEntityWithLifetime(0.0, 5.0f);
            _entityManager.AddComponent<Prefab>(prefab);

            Assert.IsTrue(_entityManager.HasComponent<Lifetime>(prefab), "Prefab should still have Lifetime component");
            Assert.IsTrue(_entityManager.HasComponent<Prefab>(prefab), "Entity should have Prefab tag");
        }

        [Test]
        public void MultipleQueries_ReturnsConsistentResults()
        {
            Entity entity1 = CreateEntityWithLifetime(0.0, 5.0f);
            Entity entity2 = CreateEntityWithLifetime(1.0, 3.0f);

            var query1 = _entityManager.CreateEntityQuery(typeof(Lifetime));
            var query2 = _entityManager.CreateEntityQuery(typeof(Lifetime));

            int count1 = query1.CalculateEntityCount();
            int count2 = query2.CalculateEntityCount();

            Assert.AreEqual(count1, count2, "Multiple queries should return same count");
            Assert.AreEqual(2, count1, "Should find both entities");
        }

        private Entity CreateEntityWithLifetime(double spawnTime, float duration)
        {
            Entity entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new Lifetime
            {
                SpawnTime = spawnTime,
                Duration = duration
            });
            return entity;
        }
    }
}

