using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Spellwright.Components.Common;

namespace Spellwright.Jobs.Lifecycle
{
    [BurstCompile]
    public partial struct DestroyExpiredSpellsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public double CurrentTime;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in Lifetime lifetime)
        {
            if (CurrentTime > lifetime.SpawnTime + lifetime.Duration)
            {
                ECB.DestroyEntity(sortKey, entity);
            }
        }
    }
}

