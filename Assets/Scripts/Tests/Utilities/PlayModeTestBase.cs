using NUnit.Framework;
using Unity.Core;
using Unity.Entities;
using UnityEngine;
using System.Collections;
using Spellwright.Systems.Lifecycle;
using Spellwright.Systems.Movement;
using Spellwright.Systems.Spawning;

namespace Spellwright.Tests.Utilities
{
    public abstract class PlayModeTestBase
    {
        protected World World;
        protected EntityManager EntityManager;
        protected InitializationSystemGroup InitializationGroup;
        protected SimulationSystemGroup SimulationGroup;

        private double _currentElapsedTime;

        [SetUp]
        public void BaseSetUp()
        {
            World = new World("TestWorld");
            EntityManager = World.EntityManager;
            _currentElapsedTime = 0.0;

            InitializationGroup = World.GetOrCreateSystemManaged<InitializationSystemGroup>();
            SimulationGroup = World.GetOrCreateSystemManaged<SimulationSystemGroup>();

            var beginInitECB = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            var validationSystem = World.CreateSystem<SpawnRequestValidationSystem>();
            var spawnSystem = World.CreateSystem<SpellSpawnSystem>();
            var movementSystem = World.CreateSystem<SpellMovementSystem>();
            var lifecycleSystem = World.CreateSystem<LifecycleSystem>();
            var endSimECB = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            InitializationGroup.AddSystemToUpdateList(beginInitECB);
            InitializationGroup.AddSystemToUpdateList(validationSystem);
            InitializationGroup.AddSystemToUpdateList(spawnSystem);
            SimulationGroup.AddSystemToUpdateList(movementSystem);
            SimulationGroup.AddSystemToUpdateList(lifecycleSystem);
            SimulationGroup.AddSystemToUpdateList(endSimECB);

            InitializationGroup.SortSystems();
            SimulationGroup.SortSystems();
        }

        [TearDown]
        public void BaseTearDown()
        {
            if (World != null && World.IsCreated)
            {
                World.Dispose();
            }
        }

        protected IEnumerator AdvanceInitializationTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            _currentElapsedTime += seconds;
            World.SetTime(new TimeData(_currentElapsedTime, seconds));
            InitializationGroup.Update();
            EntityManager.CompleteAllTrackedJobs();
        }

        protected IEnumerator AdvanceSimulationTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            _currentElapsedTime += seconds;
            World.SetTime(new TimeData(_currentElapsedTime, seconds));
            SimulationGroup.Update();
            EntityManager.CompleteAllTrackedJobs();
        }
    }
}
