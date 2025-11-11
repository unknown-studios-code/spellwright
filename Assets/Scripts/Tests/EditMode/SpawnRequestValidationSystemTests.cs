using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Spawning
{
    [TestFixture]
    public class SpawnRequestValidationSystemTests : EditModeTestBase
    {

        [Test]
        public void ValidPrefab_HasAllRequiredComponents()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);

            Assert.IsTrue(EntityManager.HasComponent<SpellOwner>(prefab), "Prefab must have SpellOwner");
            Assert.IsTrue(EntityManager.HasComponent<LocalTransform>(prefab), "Prefab must have LocalTransform");
            Assert.IsTrue(EntityManager.HasComponent<Speed>(prefab), "Prefab must have Speed");
            Assert.IsTrue(EntityManager.HasComponent<Lifetime>(prefab), "Prefab must have Lifetime");
        }

        [Test]
        public void Query_FiltersSpawnRequests()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            Entity validatedRequest = EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));
            Entity unvalidatedRequest = EntityCreationUtils.CreateSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            var validatedQuery = EntityManager.CreateEntityQuery(typeof(SpawnRequest), typeof(ValidatedTag));
            var unvalidatedQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SpawnRequest>(),
                ComponentType.Exclude<ValidatedTag>()
            );

            Assert.AreEqual(1, validatedQuery.CalculateEntityCount(), "Should find validated request");
            Assert.AreEqual(1, unvalidatedQuery.CalculateEntityCount(), "Should find unvalidated request");
        }
    }
}
