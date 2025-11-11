using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Spawning
{
    [TestFixture]
    public class SpawnRequestValidationSystemTests
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
        public void SpawnRequest_CanBeCreated()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsTrue(_entityManager.Exists(request), "SpawnRequest entity should exist");
        }

        [Test]
        public void SpawnRequest_WithoutValidatedTag_CanBeQueried()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsFalse(_entityManager.HasComponent<ValidatedTag>(request), "New SpawnRequest should not have ValidatedTag");
        }

        [Test]
        public void ValidPrefab_HasAllRequiredComponents()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            Assert.IsTrue(_entityManager.HasComponent<SpellOwner>(prefab), "Prefab must have SpellOwner");
            Assert.IsTrue(_entityManager.HasComponent<Unity.Transforms.LocalTransform>(prefab), "Prefab must have LocalTransform");
            Assert.IsTrue(_entityManager.HasComponent<Speed>(prefab), "Prefab must have Speed");
            Assert.IsTrue(_entityManager.HasComponent<Lifetime>(prefab), "Prefab must have Lifetime");
        }

        [Test]
        public void InvalidPrefab_MissingSpellOwner()
        {
            Entity prefab = TestHelpers.CreateInvalidPrefab_MissingSpellOwner(_entityManager);

            Assert.IsFalse(_entityManager.HasComponent<SpellOwner>(prefab), "Invalid prefab should not have SpellOwner");
        }

        [Test]
        public void InvalidPrefab_MissingLocalTransform()
        {
            Entity prefab = TestHelpers.CreateInvalidPrefab_MissingLocalTransform(_entityManager);

            Assert.IsFalse(_entityManager.HasComponent<Unity.Transforms.LocalTransform>(prefab), "Invalid prefab should not have LocalTransform");
        }

        [Test]
        public void InvalidPrefab_MissingSpeed()
        {
            Entity prefab = TestHelpers.CreateInvalidPrefab_MissingSpeed(_entityManager);

            Assert.IsFalse(_entityManager.HasComponent<Speed>(prefab), "Invalid prefab should not have Speed");
        }

        [Test]
        public void InvalidPrefab_MissingLifetime()
        {
            Entity prefab = TestHelpers.CreateInvalidPrefab_MissingLifetime(_entityManager);

            Assert.IsFalse(_entityManager.HasComponent<Lifetime>(prefab), "Invalid prefab should not have Lifetime");
        }

        private Entity CreateSpawnRequest(Entity prefab, Entity caster, float3 position, float3 direction)
        {
            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(request, new SpawnRequest
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

