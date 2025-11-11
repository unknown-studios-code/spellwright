using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Jobs.Spawning
{
    [TestFixture]
    public class SpawnJobTests : EditModeTestBase
    {

        [Test]
        public void Prefab_HasAllRequiredComponents()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);

            Assert.IsTrue(EntityManager.HasComponent<SpellOwner>(prefab), "Prefab must have SpellOwner");
            Assert.IsTrue(EntityManager.HasComponent<Unity.Transforms.LocalTransform>(prefab), "Prefab must have LocalTransform");
            Assert.IsTrue(EntityManager.HasComponent<Speed>(prefab), "Prefab must have Speed");
            Assert.IsTrue(EntityManager.HasComponent<Lifetime>(prefab), "Prefab must have Lifetime");
        }

        [Test]
        public void Prefab_ComponentData_IsCorrect()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);

            Unity.Transforms.LocalTransform transform = EntityManager.GetComponentData<Unity.Transforms.LocalTransform>(prefab);
            Speed speed = EntityManager.GetComponentData<Speed>(prefab);
            Lifetime lifetime = EntityManager.GetComponentData<Lifetime>(prefab);

            Assert.AreEqual(1f, transform.Scale, "Prefab scale should be 1.0");
            Assert.AreEqual(10f, speed.Value, "Prefab speed should be 10.0");
            Assert.AreEqual(5f, lifetime.Duration, "Prefab lifetime should be 5.0 seconds");
        }

        [Test]
        public void SpawnRequest_ComponentData_IsCorrect()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);
            Entity caster = EntityManager.CreateEntity();
            float3 position = new float3(1, 2, 3);
            float3 direction = new float3(0, 0, 1);

            Entity request = EntityManager.CreateEntity();
            EntityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = prefab,
                CasterEntity = caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });

            SpawnRequest spawnRequest = EntityManager.GetComponentData<SpawnRequest>(request);

            Assert.AreEqual(prefab, spawnRequest.PrefabEntity, "PrefabEntity should match");
            Assert.AreEqual(caster, spawnRequest.CasterEntity, "CasterEntity should match");
            Assert.AreEqual(position, spawnRequest.SpawnPosition, "SpawnPosition should match");
            Assert.AreEqual(direction, spawnRequest.SpawnDirection, "SpawnDirection should match");
        }
    }
}
