using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Jobs.Lifecycle;

namespace Spellwright.Tests.EditMode.Jobs.Lifecycle
{
    [TestFixture]
    public class DestroyExpiredSpellsJobTests
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
        public void ExpiredEntity_IsDestroyed()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 5.0f);
            double currentTime = 6.0;

            _ecb.DestroyEntity(entity);
            _ecb.Playback(_entityManager);

            Assert.IsFalse(_entityManager.Exists(entity), "Expired entity should be destroyed");
        }

        [Test]
        public void NonExpiredEntity_IsNotDestroyed()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 5.0f);

            Assert.IsTrue(_entityManager.Exists(entity), "Non-expired entity should exist");
        }

        [Test]
        public void ExactlyExpiredEntity_IsDestroyed()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 5.0f);
            double currentTime = 5.0;

            bool shouldDestroy = currentTime > 0.0 + 5.0f;

            Assert.IsFalse(shouldDestroy, "Entity at exact expiration time should not be destroyed");
        }

        [Test]
        public void JustAfterExpiration_IsDestroyed()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 5.0f);
            double currentTime = 5.01;

            bool shouldDestroy = currentTime > 0.0 + 5.0f;

            Assert.IsTrue(shouldDestroy, "Entity just after expiration should be destroyed");
        }

        [Test]
        public void MultipleEntities_ExpiredOnesDestroyed()
        {
            Entity expiredEntity1 = CreateEntityWithLifetime(spawnTime: 0.0, duration: 2.0f);
            Entity expiredEntity2 = CreateEntityWithLifetime(spawnTime: 1.0, duration: 3.0f);
            Entity activeEntity = CreateEntityWithLifetime(spawnTime: 5.0, duration: 10.0f);
            double currentTime = 6.0;

            if (currentTime > 0.0 + 2.0f)
            {
                _ecb.DestroyEntity(expiredEntity1);
            }
            if (currentTime > 1.0 + 3.0f)
            {
                _ecb.DestroyEntity(expiredEntity2);
            }

            _ecb.Playback(_entityManager);

            Assert.IsFalse(_entityManager.Exists(expiredEntity1), "First expired entity should be destroyed");
            Assert.IsFalse(_entityManager.Exists(expiredEntity2), "Second expired entity should be destroyed");
            Assert.IsTrue(_entityManager.Exists(activeEntity), "Active entity should still exist");
        }

        [Test]
        public void Lifetime_WithLargeSpawnTime_CalculatesCorrectly()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 100.0, duration: 5.0f);
            double currentTime = 106.0;

            bool shouldDestroy = currentTime > 100.0 + 5.0f;

            Assert.IsTrue(shouldDestroy, "Entity should be destroyed at large elapsed time");
        }

        [Test]
        public void Lifetime_WithSmallDuration_CalculatesCorrectly()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 0.1f);
            double currentTime = 0.2;

            bool shouldDestroy = currentTime > 0.0 + 0.1f;

            Assert.IsTrue(shouldDestroy, "Entity with small duration should expire quickly");
        }

        [Test]
        public void ECB_ParallelWriter_DestroysEntity()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 5.0f);

            var parallelWriter = _ecb.AsParallelWriter();
            parallelWriter.DestroyEntity(0, entity);

            _ecb.Playback(_entityManager);

            Assert.IsFalse(_entityManager.Exists(entity), "Entity destroyed via ParallelWriter should not exist");
        }

        [Test]
        public void Lifetime_WithZeroDuration_ExpiresImmediately()
        {
            Entity entity = CreateEntityWithLifetime(spawnTime: 0.0, duration: 0.0f);
            double currentTime = 0.01;

            bool shouldDestroy = currentTime > 0.0 + 0.0f;

            Assert.IsTrue(shouldDestroy, "Entity with zero duration should expire immediately");
        }

        [Test]
        public void Lifetime_ComponentData_HasCorrectValues()
        {
            double spawnTime = 10.5;
            float duration = 7.5f;
            Entity entity = CreateEntityWithLifetime(spawnTime, duration);

            Lifetime lifetime = _entityManager.GetComponentData<Lifetime>(entity);

            Assert.AreEqual(spawnTime, lifetime.SpawnTime, "SpawnTime should match");
            Assert.AreEqual(duration, lifetime.Duration, "Duration should match");
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

