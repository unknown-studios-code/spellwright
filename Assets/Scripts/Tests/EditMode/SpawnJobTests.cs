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
    public class SpawnJobTests
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
        public void Prefab_HasAllRequiredComponents()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            Assert.IsTrue(_entityManager.HasComponent<SpellOwner>(prefab), "Prefab must have SpellOwner");
            Assert.IsTrue(_entityManager.HasComponent<Unity.Transforms.LocalTransform>(prefab), "Prefab must have LocalTransform");
            Assert.IsTrue(_entityManager.HasComponent<Speed>(prefab), "Prefab must have Speed");
            Assert.IsTrue(_entityManager.HasComponent<Lifetime>(prefab), "Prefab must have Lifetime");
        }

        [Test]
        public void Prefab_HasCorrectScale()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            Unity.Transforms.LocalTransform transform = _entityManager.GetComponentData<Unity.Transforms.LocalTransform>(prefab);

            Assert.AreEqual(1f, transform.Scale, "Prefab scale should be 1.0");
        }

        [Test]
        public void Prefab_HasCorrectSpeed()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            Speed speed = _entityManager.GetComponentData<Speed>(prefab);

            Assert.AreEqual(10f, speed.Value, "Prefab speed should be 10.0");
        }

        [Test]
        public void Prefab_HasCorrectDuration()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);

            Lifetime lifetime = _entityManager.GetComponentData<Lifetime>(prefab);

            Assert.AreEqual(5f, lifetime.Duration, "Prefab lifetime should be 5.0 seconds");
        }

        [Test]
        public void SpawnRequest_HasCorrectData()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            float3 position = new float3(1, 2, 3);
            float3 direction = new float3(0, 0, 1);

            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });

            SpawnRequest spawnRequest = _entityManager.GetComponentData<SpawnRequest>(request);

            Assert.AreEqual(prefab, spawnRequest.PrefabEntity, "PrefabEntity should match");
            Assert.AreEqual(caster, spawnRequest.CasterEntity, "CasterEntity should match");
            Assert.AreEqual(position, spawnRequest.SpawnPosition, "SpawnPosition should match");
            Assert.AreEqual(direction, spawnRequest.SpawnDirection, "SpawnDirection should match");
        }

        [Test]
        public void ECB_AfterPlayback_InstantiatesEntity()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            _ecb.Instantiate(prefab);

            _ecb.Playback(_entityManager);

            int entityCount = _entityManager.CreateEntityQuery(typeof(SpellOwner)).CalculateEntityCount();
            Assert.AreEqual(2, entityCount, "Should have prefab + instantiated entity");
        }

        [Test]
        public void ECB_AfterPlayback_SetsComponent()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity caster = _entityManager.CreateEntity();
            Entity deferredSpell = _ecb.Instantiate(prefab);

            _ecb.SetComponent(deferredSpell, new SpellOwner { OwnerEntity = caster });

            _ecb.Playback(_entityManager);

            var query = _entityManager.CreateEntityQuery(typeof(SpellOwner));
            var entities = query.ToEntityArray(Allocator.Temp);

            bool foundCaster = false;
            foreach (var entity in entities)
            {
                if (entity != prefab && entity != caster)
                {
                    SpellOwner owner = _entityManager.GetComponentData<SpellOwner>(entity);
                    if (owner.OwnerEntity == caster)
                    {
                        foundCaster = true;
                        break;
                    }
                }
            }
            entities.Dispose();

            Assert.IsTrue(foundCaster, "Should find entity with caster as owner");
        }

        [Test]
        public void ECB_AfterPlayback_AddsComponent()
        {
            Entity prefab = TestHelpers.CreateValidPrefab(_entityManager);
            Entity deferredSpell = _ecb.Instantiate(prefab);

            _ecb.AddComponent(deferredSpell, new Velocity { Value = new float3(0, 0, 10) });

            _ecb.Playback(_entityManager);

            var query = _entityManager.CreateEntityQuery(typeof(Velocity));
            int velocityCount = query.CalculateEntityCount();

            Assert.AreEqual(1, velocityCount, "Should have exactly one entity with Velocity");
        }

        [Test]
        public void ECB_AfterPlayback_DestroysEntity()
        {
            Entity entity = _entityManager.CreateEntity();
            _ecb.DestroyEntity(entity);

            _ecb.Playback(_entityManager);

            Assert.IsFalse(_entityManager.Exists(entity), "Entity should be destroyed after playback");
        }
    }
}

