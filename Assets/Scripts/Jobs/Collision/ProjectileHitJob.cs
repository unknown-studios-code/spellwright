using Spellwright.Components.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using CollisionEvent = Spellwright.Components.Collision.CollisionEvent;
using HealthComponent = Spellwright.Components.Health;

namespace Spellwright.Jobs.Collision
{
    [BurstCompile]
    public struct ProjectileHitJob : ITriggerEventsJob
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public ComponentLookup<ProjectileTag> ProjectileLookup;

        [ReadOnly]
        public ComponentLookup<HealthComponent> HealthLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            if (!TryGetProjectileAndTarget(triggerEvent.EntityA, triggerEvent.EntityB, out Entity projectile, out Entity target))
            {
                return;
            }

            float3 impactPosition = GetImpactPosition(target);
            CreateCollisionEvent(projectile, target, impactPosition);
        }

        [BurstCompile]
        private bool TryGetProjectileAndTarget(Entity entityA, Entity entityB, out Entity projectile, out Entity target)
        {
            bool isAProjectile = ProjectileLookup.HasComponent(entityA);
            bool isBProjectile = ProjectileLookup.HasComponent(entityB);
            bool isADamageable = HealthLookup.HasComponent(entityA);
            bool isBDamageable = HealthLookup.HasComponent(entityB);

            if (isAProjectile && isBDamageable)
            {
                projectile = entityA;
                target = entityB;
                return true;
            }

            if (isBProjectile && isADamageable)
            {
                projectile = entityB;
                target = entityA;
                return true;
            }

            projectile = Entity.Null;
            target = Entity.Null;
            return false;
        }

        [BurstCompile]
        private float3 GetImpactPosition(Entity target)
        {
            if (TransformLookup.HasComponent(target))
            {
                return TransformLookup[target].Position;
            }
            return float3.zero;
        }

        [BurstCompile]
        private void CreateCollisionEvent(Entity projectile, Entity target, float3 impactPosition)
        {
            ECB.AddComponent(
                projectile.Index,
                projectile,
                new CollisionEvent
                {
                    ProjectileEntity = projectile,
                    TargetEntity = target,
                    ImpactPosition = impactPosition,
                }
            );
        }
    }
}
