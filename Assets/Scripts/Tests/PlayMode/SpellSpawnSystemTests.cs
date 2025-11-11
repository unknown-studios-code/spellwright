using NUnit.Framework;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Collections;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Systems.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.PlayMode.Systems.Spawning
{
    [TestFixture]
    public class SpellSpawnSystemTests : PlayModeTestBase
    {
        [UnityTest]
        public IEnumerator SpellSpawn_ValidRequest_CreatesSpellAfterOneFrame()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(Velocity));
            int spellCount = query.CalculateEntityCount();

            Assert.AreEqual(1, spellCount, "Should spawn one spell after frame");
        }

        [UnityTest]
        public IEnumerator SpellSpawn_CreatesSpellWithVelocity()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            float3 direction = new float3(1, 0, 0);
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, direction);

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(Velocity));
            var entities = query.ToEntityArray(Allocator.Temp);

            Assert.AreEqual(1, entities.Length, "Should have one spell");

            Velocity velocity = EntityManager.GetComponentData<Velocity>(entities[0]);
            Assert.Greater(math.length(velocity.Value), 0, "Spell should have velocity");

            entities.Dispose();
        }

        [UnityTest]
        public IEnumerator SpellSpawn_MultipleRequests_CreatesMultipleSpells()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();

            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, new float3(0, 0, 0), new float3(1, 0, 0));

            Entity prefab2 = EntityCreationUtils.CreateValidPrefab(EntityManager);
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab2, caster, new float3(5, 0, 0), new float3(0, 1, 0));

            Entity prefab3 = EntityCreationUtils.CreateValidPrefab(EntityManager);
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab3, caster, new float3(0, 5, 0), new float3(0, 0, 1));

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(Velocity));
            int spellCount = query.CalculateEntityCount();

            Assert.AreEqual(3, spellCount, "Should spawn 3 spells");
        }

        [UnityTest]
        public IEnumerator SpellSpawn_RequestDestroyed_AfterProcessing()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            Entity request = EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            Assert.IsFalse(EntityManager.Exists(request), "SpawnRequest should be destroyed after processing");
        }

        [UnityTest]
        public IEnumerator SpellSpawn_SetsCorrectOwner()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, float3.zero, new float3(0, 0, 1));

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(SpellOwner), typeof(Velocity));
            var entities = query.ToEntityArray(Allocator.Temp);

            Assert.AreEqual(1, entities.Length, "Should have one spell");

            SpellOwner owner = EntityManager.GetComponentData<SpellOwner>(entities[0]);
            Assert.AreEqual(caster, owner.OwnerEntity, "Spell owner should be caster");

            entities.Dispose();
        }

        [UnityTest]
        public IEnumerator SpellSpawn_SetsCorrectPosition()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            float3 spawnPosition = new float3(10, 20, 30);
            EntityCreationUtils.CreateValidatedSpawnRequest(EntityManager, prefab, caster, spawnPosition, new float3(0, 0, 1));

            yield return AdvanceInitializationTime(0.016f);
            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(LocalTransform), typeof(Velocity));
            var entities = query.ToEntityArray(Allocator.Temp);

            Assert.AreEqual(1, entities.Length, "Should have one spell");

            LocalTransform transform = EntityManager.GetComponentData<LocalTransform>(entities[0]);
            Assert.That(transform.Position.x, Is.EqualTo(spawnPosition.x).Within(0.01f), "X position should match");
            Assert.That(transform.Position.y, Is.EqualTo(spawnPosition.y).Within(0.01f), "Y position should match");
            Assert.That(transform.Position.z, Is.EqualTo(spawnPosition.z).Within(0.01f), "Z position should match");

            entities.Dispose();
        }

        [UnityTest]
        public IEnumerator SpellSpawn_WithoutValidatedTag_NotProcessed()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();

            Entity request = EntityManager.CreateEntity();
            EntityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = float3.zero,
                SpawnDirection = new float3(0, 0, 1)
            });

            yield return AdvanceInitializationTime(0.016f);

            var query = EntityManager.CreateEntityQuery(typeof(Velocity));
            int spellCount = query.CalculateEntityCount();

            Assert.AreEqual(0, spellCount, "Should not spawn spell without ValidatedTag");
        }
    }
}
