using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Spellwright.Components.Common;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Systems.Movement
{
    [TestFixture]
    public class SpellMovementSystemTests : EditModeTestBase
    {

        [Test]
        public void Query_RequiresBothVelocityAndTransform()
        {
            Entity withBoth = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            Entity onlyVelocity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(onlyVelocity, new Velocity { Value = new float3(10, 0, 0) });
            Entity onlyTransform = EntityManager.CreateEntity();
            EntityManager.AddComponentData(onlyTransform, LocalTransform.FromPosition(float3.zero));

            var query = EntityManager.CreateEntityQuery(typeof(Velocity), typeof(LocalTransform));

            Assert.AreEqual(1, query.CalculateEntityCount(), "Query should only return entities with both components");
        }

        [Test]
        public void Query_ExcludesPrefabs()
        {
            Entity regularEntity = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            Entity prefabEntity = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(5, 5, 5), new float3(10, 10, 10));
            EntityManager.AddComponent<Prefab>(prefabEntity);

            var query = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Velocity>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.Exclude<Prefab>()
            );

            Assert.AreEqual(1, query.CalculateEntityCount(), "Query should exclude prefab entities");
        }

        [Test]
        public void Velocity_ComponentData_IsCorrect()
        {
            float3 velocityValue = new float3(10, 20, 30);
            Entity entity = EntityCreationUtils.CreateMovingEntity(EntityManager, velocityValue, float3.zero);

            Velocity velocity = EntityManager.GetComponentData<Velocity>(entity);

            Assert.AreEqual(velocityValue, velocity.Value, "Velocity component should contain correct data");
        }
    }
}

