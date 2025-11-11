using NUnit.Framework;
using Unity.Entities;
using Spellwright.Components.Common;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Lifecycle
{
    [TestFixture]
    public class LifecycleSystemTests : EditModeTestBase
    {

        [Test]
        public void Query_RequiresLifetime()
        {
            Entity withLifetime = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, 0.0, 5.0f);
            Entity withoutLifetime = EntityManager.CreateEntity();

            var query = EntityManager.CreateEntityQuery(typeof(Lifetime));

            Assert.AreEqual(1, query.CalculateEntityCount(), "Query should only return entities with Lifetime");
        }

        [Test]
        public void Query_ExcludesPrefabs()
        {
            Entity regularEntity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, 0.0, 5.0f);
            Entity prefabEntity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, 1.0, 3.0f);
            EntityManager.AddComponent<Prefab>(prefabEntity);

            var query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Lifetime>(),
                ComponentType.Exclude<Prefab>()
            );

            Assert.AreEqual(1, query.CalculateEntityCount(), "Query should exclude prefab entities");
        }

        [Test]
        public void Lifetime_ComponentData_IsCorrect()
        {
            double spawnTime = 10.5;
            float duration = 7.5f;
            Entity entity = EntityCreationUtils.CreateEntityWithLifetime(EntityManager, spawnTime, duration);

            Lifetime lifetime = EntityManager.GetComponentData<Lifetime>(entity);

            Assert.AreEqual(spawnTime, lifetime.SpawnTime, "SpawnTime should match");
            Assert.AreEqual(duration, lifetime.Duration, "Duration should match");
        }
    }
}
