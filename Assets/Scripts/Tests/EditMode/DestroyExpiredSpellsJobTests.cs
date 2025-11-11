using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Spellwright.Components.Common;
using Spellwright.Jobs.Lifecycle;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Jobs.Lifecycle
{
    [TestFixture]
    public class DestroyExpiredSpellsJobTests : EditModeTestBase
    {

        [Test]
        public void Lifetime_ComponentData_HasCorrectValues()
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

