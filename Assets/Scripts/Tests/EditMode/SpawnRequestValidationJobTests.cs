using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Jobs.Spawning
{
    [TestFixture]
    public class SpawnRequestValidationJobTests
    {
        private World _world;
        private EntityManager _entityManager;
        private EntityCommandBuffer _ecb;

        [SetUp]
        public void SetUp()
        {
            _world = new World("TestWorld");
            _entityManager = _world.EntityManager;
            _ecb = new EntityCommandBuffer(Allocator.TempJob);
        }

        [TearDown]
        public void TearDown()
        {
            if (_ecb.IsCreated)
            {
                _ecb.Dispose();
            }

            if (_world != null && _world.IsCreated)
            {
                _world.Dispose();
            }
        }

        [Test]
        public void ValidPrefab_HasAllComponents()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            bool hasAllComponents = _entityManager.HasComponent<SpellOwner>(prefab)
                && _entityManager.HasComponent<Unity.Transforms.LocalTransform>(prefab)
                && _entityManager.HasComponent<Speed>(prefab)
                && _entityManager.HasComponent<Lifetime>(prefab);

            Assert.IsTrue(hasAllComponents, "Valid prefab must have all required components");
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

        [Test]
        public void ECB_AfterPlayback_AddsValidatedTag()
        {
            Entity request = _entityManager.CreateEntity();
            _ecb.AddComponent<ValidatedTag>(request);

            _ecb.Playback(_entityManager);

            Assert.IsTrue(_entityManager.HasComponent<ValidatedTag>(request), "Entity should have ValidatedTag after playback");
        }

        [Test]
        public void ECB_AfterPlayback_DestroysInvalidRequest()
        {
            Entity request = _entityManager.CreateEntity();
            _ecb.DestroyEntity(request);

            _ecb.Playback(_entityManager);

            Assert.IsFalse(_entityManager.Exists(request), "Invalid request should be destroyed after playback");
        }

        [Test]
        public void SpawnRequest_CanBeCreated()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();

            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = float3.zero,
                SpawnDirection = new float3(0, 0, 1)
            });

            Assert.IsTrue(_entityManager.HasComponent<SpawnRequest>(request), "Entity should have SpawnRequest component");
        }

        [Test]
        public void SpawnRequest_WithoutValidatedTag()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();

            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = float3.zero,
                SpawnDirection = new float3(0, 0, 1)
            });

            Assert.IsFalse(_entityManager.HasComponent<ValidatedTag>(request), "New SpawnRequest should not have ValidatedTag");
        }
    }
}

