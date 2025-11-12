using Spellwright.Components.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Spellwright.Jobs.Collision
{
    [BurstCompile]
    public struct ProjectileHitJob : ITriggerEventsJob
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public ComponentLookup<ProjectileTag> ProjectileLookup;

        [ReadOnly]
        public ComponentLookup<DamageableTag> DamageableLookup;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isAProjectile = ProjectileLookup.HasComponent(entityA);
            bool isBProjectile = ProjectileLookup.HasComponent(entityB);
            bool isADamageable = DamageableLookup.HasComponent(entityA);
            bool isBDamageable = DamageableLookup.HasComponent(entityB);

            Entity projectile = Entity.Null;
            Entity target = Entity.Null;

            if (isAProjectile && isBDamageable)
            {
                projectile = entityA;
                target = entityB;
            }
            else if (isBProjectile && isADamageable)
            {
                projectile = entityB;
                target = entityA;
            }
            else
            {
                return;
            }

            int sortKey = projectile.Index;
            Entity collisionEventEntity = ECB.CreateEntity(sortKey);

            ECB.AddComponent(
                sortKey,
                collisionEventEntity,
                new Components.Collision.CollisionEvent
                {
                    ProjectileEntity = projectile,
                    TargetEntity = target,
                    ImpactPosition = float3.zero,
                }
            );
            ECB.DestroyEntity(sortKey, projectile);
        }
    }
}
