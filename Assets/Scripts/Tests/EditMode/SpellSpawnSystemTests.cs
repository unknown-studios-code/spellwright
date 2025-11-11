using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Spawning
{
    [TestFixture]
    public class SpellSpawnSystemTests
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
        public void SpawnRequest_CanBeCreatedWithValidatedTag()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsTrue(_entityManager.Exists(request), "SpawnRequest entity should exist");
        }

        [Test]
        public void ValidatedSpawnRequest_HasValidatedTag()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsTrue(_entityManager.HasComponent<ValidatedTag>(request), "SpawnRequest must have ValidatedTag");
        }

        [Test]
        public void ValidatedSpawnRequest_HasSpawnRequest()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsTrue(_entityManager.HasComponent<SpawnRequest>(request), "Entity must have SpawnRequest component");
        }

        [Test]
        public void SpawnRequest_HasCorrectPrefabEntity()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            SpawnRequest spawnRequest = _entityManager.GetComponentData<SpawnRequest>(request);
            Assert.AreEqual(prefab, spawnRequest.PrefabEntity, "PrefabEntity should match the created prefab");
        }

        [Test]
        public void SpawnRequest_HasCorrectCasterEntity()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, new float3(0, 0, 1));

            SpawnRequest spawnRequest = _entityManager.GetComponentData<SpawnRequest>(request);
            Assert.AreEqual(caster, spawnRequest.CasterEntity, "CasterEntity should match the created caster");
        }

        [Test]
        public void SpawnRequest_HasCorrectPosition()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            float3 position = new float3(1, 2, 3);
            Entity request = CreateValidatedSpawnRequest(prefab, caster, position, new float3(0, 0, 1));

            SpawnRequest spawnRequest = _entityManager.GetComponentData<SpawnRequest>(request);
            Assert.AreEqual(position, spawnRequest.SpawnPosition, "SpawnPosition should match the specified position");
        }

        [Test]
        public void SpawnRequest_HasCorrectDirection()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            float3 direction = new float3(1, 0, 0);
            Entity request = CreateValidatedSpawnRequest(prefab, caster, float3.zero, direction);

            SpawnRequest spawnRequest = _entityManager.GetComponentData<SpawnRequest>(request);
            Assert.AreEqual(direction, spawnRequest.SpawnDirection, "SpawnDirection should match the specified direction");
        }

        private Entity CreateValidatedSpawnRequest(Entity prefab, Entity caster, float3 position, float3 direction)
        {
            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });
            _entityManager.AddComponent<ValidatedTag>(request);

            return request;
        }
    }
}

