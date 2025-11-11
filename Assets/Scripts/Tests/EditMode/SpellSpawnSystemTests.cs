using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Spawning
{
    [TestFixture]
    public class SpellSpawnSystemTests : EditModeTestBase
    {

        [Test]
        public void SpawnRequest_ComponentData_IsCorrect()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            float3 position = new float3(1, 2, 3);
            float3 direction = new float3(0, 0, 1);
            Entity request = EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, position, direction);

            SpawnRequest spawnRequest = EntityManager.GetComponentData<SpawnRequest>(request);

            Assert.AreEqual(prefab, spawnRequest.PrefabEntity, "PrefabEntity should match");
            Assert.AreEqual(caster, spawnRequest.CasterEntity, "CasterEntity should match");
            Assert.AreEqual(position, spawnRequest.SpawnPosition, "SpawnPosition should match");
            Assert.AreEqual(direction, spawnRequest.SpawnDirection, "SpawnDirection should match");
        }

        [Test]
        public void ValidatedTag_CanBeAdded()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            Entity request = EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            Assert.IsTrue(EntityManager.HasComponent<ValidatedTag>(request), "ValidatedTag should be present");
        }

        [Test]
        public void SpawnRequest_RequiresPrefabAndCaster()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            Entity request = EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            SpawnRequest spawnRequest = EntityManager.GetComponentData<SpawnRequest>(request);
            Assert.IsTrue(EntityManager.Exists(spawnRequest.PrefabEntity), "Prefab entity should exist");
            Assert.IsTrue(EntityManager.Exists(spawnRequest.CasterEntity), "Caster entity should exist");
        }
    }
}
