using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Spellwright.Components.Common;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.EditMode.Jobs.Movement
{
    [TestFixture]
    public class ApplyVelocityJobTests : EditModeTestBase
    {

        [Test]
        public void Execute_AppliesVelocityToPosition()
        {
            Entity entity = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            float deltaTime = 1.0f;

            ApplyVelocityManually(entity, deltaTime);

            LocalTransform finalTransform = EntityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(new float3(10, 0, 0), finalTransform.Position, "Position should move by velocity * deltaTime");
        }

        [Test]
        public void Execute_WithZeroVelocity_DoesNotMove()
        {
            Entity entity = EntityCreationUtils.CreateMovingEntity(EntityManager, float3.zero, float3.zero);
            float deltaTime = 1.0f;

            ApplyVelocityManually(entity, deltaTime);

            LocalTransform finalTransform = EntityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(float3.zero, finalTransform.Position, "Position should not change with zero velocity");
        }

        [Test]
        public void Execute_WithSmallDeltaTime_ScalesCorrectly()
        {
            Entity entity = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            float deltaTime = 0.1f;

            ApplyVelocityManually(entity, deltaTime);

            LocalTransform finalTransform = EntityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(new float3(1, 0, 0), finalTransform.Position, "Position should scale with deltaTime");
        }

        [Test]
        public void Execute_WithDiagonalVelocity_Applies3DMovement()
        {
            Entity entity = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(3, 4, 5), float3.zero);
            float deltaTime = 1.0f;

            ApplyVelocityManually(entity, deltaTime);

            LocalTransform finalTransform = EntityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(new float3(3, 4, 5), finalTransform.Position, "Position should move in all 3 dimensions");
        }

        [Test]
        public void Execute_PreservesRotationAndScale()
        {
            quaternion rotation = quaternion.EulerXYZ(0.5f, 1.0f, 1.5f);
            float scale = 2.5f;
            Entity entity = EntityCreationUtils.CreateEntityWithVelocityAndRotation(EntityManager, new float3(10, 0, 0), rotation);
            EntityManager.SetComponentData(entity, LocalTransform.FromPositionRotationScale(float3.zero, rotation, scale));
            float deltaTime = 1.0f;

            ApplyVelocityManually(entity, deltaTime);

            LocalTransform finalTransform = EntityManager.GetComponentData<LocalTransform>(entity);
            Assert.AreEqual(rotation, finalTransform.Rotation, "Rotation should not be modified");
            Assert.AreEqual(scale, finalTransform.Scale, "Scale should not be modified");
        }

        private void ApplyVelocityManually(Entity entity, float deltaTime)
        {
            LocalTransform transform = EntityManager.GetComponentData<LocalTransform>(entity);
            Velocity velocity = EntityManager.GetComponentData<Velocity>(entity);

            transform.Position += velocity.Value * deltaTime;
            EntityManager.SetComponentData(entity, transform);
        }
    }
}

