using Spellwright.Components.Collision;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Collision
{
    [BurstCompile]
    public partial struct DestroyTargetJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in CollisionEvent collisionEvent)
        {
            ECB.DestroyEntity(sortKey, collisionEvent.TargetEntity);
            ECB.DestroyEntity(sortKey, entity);
        }
    }
}
