using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Tests.Utilities
{
    public abstract class EditModeTestBase
    {
        protected World World;
        protected EntityManager EntityManager;
        protected EntityCommandBuffer ECB;

        [SetUp]
        public void BaseSetUp()
        {
            World = new World("TestWorld");
            EntityManager = World.EntityManager;
            ECB = new EntityCommandBuffer(Allocator.TempJob);
        }

        [TearDown]
        public void BaseTearDown()
        {
            if (ECB.IsCreated)
            {
                ECB.Dispose();
            }

            if (World != null && World.IsCreated)
            {
                World.Dispose();
            }
        }
    }
}

