using Spellwright.Components.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using HealthComponent = Spellwright.Components.Common.Health;

namespace Spellwright.Jobs.Collision
{
    [BurstCompile]
    public struct DetectProjectileCollisionJob : ITriggerEventsJob
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
            Entity projectile = GetProjectile(triggerEvent.EntityA, triggerEvent.EntityB);

            if (projectile == Entity.Null)
            {
                return;
            }

            Entity other = projectile == triggerEvent.EntityA ? triggerEvent.EntityB : triggerEvent.EntityA;

            if (HealthLookup.HasComponent(other))
            {
                ECB.AppendToBuffer(projectile.Index, projectile, new CollisionHit { TargetEntity = other, ImpactPosition = GetImpactPosition(other) });
            }
            else
            {
                ECB.DestroyEntity(projectile.Index, projectile);
            }
        }

        [BurstCompile]
        private Entity GetProjectile(Entity entityA, Entity entityB)
        {
            return ProjectileLookup.HasComponent(entityA) ? entityA
                : ProjectileLookup.HasComponent(entityB) ? entityB
                : Entity.Null;
        }

        [BurstCompile]
        private float3 GetImpactPosition(Entity target)
        {
            return TransformLookup.HasComponent(target) ? TransformLookup[target].Position : float3.zero;
        }
    }
}
