using NUnit.Framework;
using UnityEngine.TestTools;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections;
using Spellwright.Components.Common;
using Spellwright.Systems.Movement;
using Spellwright.Tests.Utilities;

namespace Spellwright.Tests.PlayMode.Systems.Movement
{
    [TestFixture]
    public class SpellMovementSystemTests : PlayModeTestBase
    {
        [UnityTest]
        public IEnumerator SpellMovement_AfterOneFrame_SpellMoves()
        {
            Entity spell = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            float3 startPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;

            yield return AdvanceSimulationTime(0.016f);

            float3 endPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;
            float distance = math.distance(startPos, endPos);

            Assert.Greater(distance, 0.01f, "Spell should have moved after 1 frame");
        }

        [UnityTest]
        public IEnumerator SpellMovement_AfterThreeFrames_AccumulatesMovement()
        {
            Entity spell = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(1, 0, 0), float3.zero);
            float3 startPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;

            yield return AdvanceSimulationTime(0.016f);
            yield return AdvanceSimulationTime(0.016f);
            yield return AdvanceSimulationTime(0.016f);

            float3 endPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;
            float distance = math.distance(startPos, endPos);

            Assert.Greater(distance, 0.045f, "Spell should accumulate movement over 3 frames");
        }

        [UnityTest]
        public IEnumerator SpellMovement_MultipleSpells_MoveIndependently()
        {
            Entity spell1 = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), new float3(0, 0, 0));
            Entity spell2 = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(0, 10, 0), new float3(5, 5, 5));
            Entity spell3 = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(5, 5, 5), new float3(-10, 0, 10));

            float3 start1 = EntityManager.GetComponentData<LocalTransform>(spell1).Position;
            float3 start2 = EntityManager.GetComponentData<LocalTransform>(spell2).Position;
            float3 start3 = EntityManager.GetComponentData<LocalTransform>(spell3).Position;

            yield return AdvanceSimulationTime(0.016f);

            float3 end1 = EntityManager.GetComponentData<LocalTransform>(spell1).Position;
            float3 end2 = EntityManager.GetComponentData<LocalTransform>(spell2).Position;
            float3 end3 = EntityManager.GetComponentData<LocalTransform>(spell3).Position;

            Assert.AreNotEqual(start1, end1, "Spell 1 should have moved");
            Assert.AreNotEqual(start2, end2, "Spell 2 should have moved");
            Assert.AreNotEqual(start3, end3, "Spell 3 should have moved");
        }

        [UnityTest]
        public IEnumerator SpellMovement_ZeroVelocity_DoesNotMove()
        {
            Entity spell = EntityCreationUtils.CreateMovingEntity(EntityManager, float3.zero, float3.zero);
            float3 startPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;

            yield return AdvanceSimulationTime(0.016f);

            float3 endPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;

            Assert.AreEqual(startPos, endPos, "Spell with zero velocity should not move");
        }

        [UnityTest]
        public IEnumerator SpellMovement_PrefabEntity_DoesNotMove()
        {
            Entity prefab = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(10, 0, 0), float3.zero);
            EntityManager.AddComponent<Prefab>(prefab);

            float3 startPos = EntityManager.GetComponentData<LocalTransform>(prefab).Position;

            yield return AdvanceSimulationTime(0.016f);

            float3 endPos = EntityManager.GetComponentData<LocalTransform>(prefab).Position;

            Assert.AreEqual(startPos, endPos, "Prefab entity should not move");
        }

        [UnityTest]
        public IEnumerator SpellMovement_CorrectDirection_MovesInVelocityDirection()
        {
            Entity spell = EntityCreationUtils.CreateMovingEntity(EntityManager, new float3(1, 0, 0), float3.zero);
            float3 startPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;

            yield return AdvanceSimulationTime(0.016f);

            float3 endPos = EntityManager.GetComponentData<LocalTransform>(spell).Position;
            float3 displacement = endPos - startPos;

            Assert.Greater(displacement.x, 0, "Spell should move in positive X direction");
            Assert.That(displacement.y, Is.EqualTo(0).Within(0.001f), "Spell should not move in Y direction");
            Assert.That(displacement.z, Is.EqualTo(0).Within(0.001f), "Spell should not move in Z direction");
        }

        [UnityTest]
        public IEnumerator SpellMovement_WithoutTransform_SystemHandlesGracefully()
        {
            Entity invalidSpell = EntityManager.CreateEntity();
            EntityManager.AddComponentData(invalidSpell, new Velocity { Value = new float3(10, 0, 0) });

            yield return AdvanceSimulationTime(0.016f);

            Assert.IsTrue(EntityManager.Exists(invalidSpell), "Entity without transform should still exist");
        }
    }
}
