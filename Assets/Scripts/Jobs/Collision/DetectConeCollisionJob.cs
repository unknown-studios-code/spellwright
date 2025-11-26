using Spellwright.Components.Collision;
using Spellwright.Utilities;
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
    public partial struct DetectConeCollisionJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public CollisionWorld CollisionWorld;

        [ReadOnly]
        public ComponentLookup<HealthComponent> HealthLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        public CollisionFilter EnvironmentFilter;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in ConeRequest request)
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);

            if (CollisionWorld.OverlapSphere(request.Position, request.Radius, ref hits, EnvironmentFilter))
            {
                float3 coneForward = math.normalize(request.Direction);
                float cosHalfAngle = math.cos(math.radians(request.AngleDegrees * 0.5f));

                DynamicBuffer<CollisionHit> buffer = ECB.AddBuffer<CollisionHit>(sortKey, entity);

                for (int i = 0; i < hits.Length; i++)
                {
                    DistanceHit hit = hits[i];

                    if (!IsValidTarget(hit.Entity, request.SourceEntity))
                    {
                        continue;
                    }

                    if (!IsInConeArea(hit.Entity, request.Position, coneForward, cosHalfAngle))
                    {
                        continue;
                    }

                    if (!HasLineOfSight(hit.Entity, request.Position, request.CheckLineOfSight))
                    {
                        continue;
                    }

                    float3 targetPosition = TransformLookup[hit.Entity].Position;
                    buffer.Add(new CollisionHit { TargetEntity = hit.Entity, ImpactPosition = targetPosition });
                }
            }

            hits.Dispose();
            ECB.RemoveComponent<ConeRequest>(sortKey, entity);
        }

        [BurstCompile]
        private bool IsValidTarget(Entity target, Entity sourceEntity)
        {
            return HealthLookup.HasComponent(target) && target != sourceEntity && TransformLookup.HasComponent(target);
        }

        [BurstCompile]
        private bool IsInConeArea(Entity target, float3 coneOrigin, float3 coneForward, float cosHalfAngle)
        {
            float3 targetPosition = TransformLookup[target].Position;
            return CollisionUtils.IsInCone(coneOrigin, coneForward, cosHalfAngle, targetPosition);
        }

        [BurstCompile]
        private readonly bool HasLineOfSight(Entity target, float3 origin, bool checkLineOfSight)
        {
            return !checkLineOfSight || CollisionUtils.HasLineOfSight(CollisionWorld, origin, target, TransformLookup);
        }
    }
}
