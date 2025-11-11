using NUnit.Framework;
using Unity.Entities;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Tests.Utilities;
using Unity.Transforms;

namespace Spellwright.Tests.EditMode.Jobs.Spawning
{
    [TestFixture]
    public class SpawnRequestValidationJobTests : EditModeTestBase
    {

        [Test]
        public void IsValidPrefab_WithAllComponents_ReturnsTrue()
        {
            Entity prefab = EntityCreationUtils.CreateValidPrefab(EntityManager);

            bool isValid = ValidatesPrefabManually(prefab);

            Assert.IsTrue(isValid, "Prefab with all required components should be valid");
        }

        [Test]
        public void IsValidPrefab_MissingSpellOwner_ReturnsFalse()
        {
            Entity prefab = EntityCreationUtils.CreateInvalidPrefab_MissingSpellOwner(EntityManager);

            bool isValid = ValidatesPrefabManually(prefab);

            Assert.IsFalse(isValid, "Prefab missing SpellOwner should be invalid");
        }

        [Test]
        public void IsValidPrefab_MissingLocalTransform_ReturnsFalse()
        {
            Entity prefab = EntityCreationUtils.CreateInvalidPrefab_MissingLocalTransform(EntityManager);

            bool isValid = ValidatesPrefabManually(prefab);

            Assert.IsFalse(isValid, "Prefab missing LocalTransform should be invalid");
        }

        [Test]
        public void IsValidPrefab_MissingSpeed_ReturnsFalse()
        {
            Entity prefab = EntityCreationUtils.CreateInvalidPrefab_MissingSpeed(EntityManager);

            bool isValid = ValidatesPrefabManually(prefab);

            Assert.IsFalse(isValid, "Prefab missing Speed should be invalid");
        }

        [Test]
        public void IsValidPrefab_MissingLifetime_ReturnsFalse()
        {
            Entity prefab = EntityCreationUtils.CreateInvalidPrefab_MissingLifetime(EntityManager);

            bool isValid = ValidatesPrefabManually(prefab);

            Assert.IsFalse(isValid, "Prefab missing Lifetime should be invalid");
        }

        private bool ValidatesPrefabManually(Entity prefabEntity)
        {
            return EntityManager.HasComponent<SpellOwner>(prefabEntity)
                && EntityManager.HasComponent<LocalTransform>(prefabEntity)
                && EntityManager.HasComponent<Speed>(prefabEntity)
                && EntityManager.HasComponent<Lifetime>(prefabEntity);
        }
    }
}

