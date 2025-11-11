using NUnit.Framework;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Transforms;
using System.Collections;
using Spellwright.Components.Common;
using Spellwright.Systems.Lifecycle;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.PlayMode.Systems.Lifecycle
{
    [TestFixture]
    public class LifecycleSystemTests : PlayModeTestBase
    {
        [UnityTest]
        public IEnumerator Lifecycle_ExpiredEntity_DestroyedAfterUpdate()
        {
            Entity entity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 1.0f);

            yield return AdvanceSimulationTime(1.5f);

            Assert.IsFalse(EntityManager.Exists(entity), "Expired entity should be destroyed");
        }

        [UnityTest]
        public IEnumerator Lifecycle_NonExpiredEntity_RemainsAlive()
        {
            Entity entity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 5.0f);

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsTrue(EntityManager.Exists(entity), "Non-expired entity should still exist");
        }

        [UnityTest]
        public IEnumerator Lifecycle_MultipleEntities_ExpireAtCorrectTime()
        {
            Entity shortLife = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);
            Entity mediumLife = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 1.5f);
            Entity longLife = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 3.0f);

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsFalse(EntityManager.Exists(shortLife), "Short lifetime entity should be destroyed");
            Assert.IsTrue(EntityManager.Exists(mediumLife), "Medium lifetime entity should still exist");
            Assert.IsTrue(EntityManager.Exists(longLife), "Long lifetime entity should still exist");

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsFalse(EntityManager.Exists(mediumLife), "Medium lifetime entity should now be destroyed");
            Assert.IsTrue(EntityManager.Exists(longLife), "Long lifetime entity should still exist");
        }

        [UnityTest]
        public IEnumerator Lifecycle_MultipleEntities_CorrectlyDestroyed()
        {
            Entity entity1 = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);
            Entity entity2 = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 1.5f);

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsFalse(EntityManager.Exists(entity1), "Entity with 0.5s lifetime should be destroyed");
            Assert.IsTrue(EntityManager.Exists(entity2), "Entity with 1.5s lifetime should remain alive");
        }

        [UnityTest]
        public IEnumerator Lifecycle_PrefabEntity_NotDestroyed()
        {
            Entity prefab = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);
            EntityManager.AddComponent<Prefab>(prefab);

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsTrue(EntityManager.Exists(prefab), "Prefab entity should not be destroyed");
        }

        [UnityTest]
        public IEnumerator Lifecycle_QueryCount_DecreasesAfterDestruction()
        {
            EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);
            EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);
            EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.5f);

            var query = EntityManager.CreateEntityQuery(typeof(Lifetime));
            int initialCount = query.CalculateEntityCount();

            Assert.AreEqual(3, initialCount, "Should have 3 entities initially");

            yield return AdvanceSimulationTime(1.0f);

            int finalCount = query.CalculateEntityCount();

            Assert.AreEqual(0, finalCount, "All entities should be destroyed");
        }

        [UnityTest]
        public IEnumerator Lifecycle_VeryShortLifetime_DestroyedQuickly()
        {
            Entity entity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 0.1f);

            yield return AdvanceSimulationTime(0.2f);

            Assert.IsFalse(EntityManager.Exists(entity), "Entity with very short lifetime should be destroyed");
        }

        [UnityTest]
        public IEnumerator Lifecycle_ExactExpiration_EntityDestroyed()
        {
            Entity entity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, World, 0.0, 1.0f);

            yield return AdvanceSimulationTime(1.0f);

            Assert.IsFalse(EntityManager.Exists(entity), "Entity should be destroyed at exact expiration time");
        }
    }
}
