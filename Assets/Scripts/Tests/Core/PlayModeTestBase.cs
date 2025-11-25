using System.Collections;
using NUnit.Framework;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

namespace Spellwright.Tests.Core
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

            World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            World.GetOrCreateSystemManaged<EndInitializationEntityCommandBufferSystem>();
            World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

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
