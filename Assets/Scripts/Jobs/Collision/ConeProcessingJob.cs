using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Utilities;
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
    public partial struct ConeProcessingJob : IJobEntity
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
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in ConeRequest request, in DynamicBuffer<PayloadRequest> payloadBuffer)
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);

            if (CollisionWorld.OverlapSphere(request.Position, request.Radius, ref hits, EnvironmentFilter))
            {
                float3 coneForward = math.normalize(request.Direction);
                float cosHalfAngle = math.cos(math.radians(request.AngleDegrees * 0.5f));

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
                    CreateCollisionEvent(sortKey, request.SourceEntity, hit.Entity, targetPosition, payloadBuffer);
                }
            }

            hits.Dispose();
            ECB.DestroyEntity(sortKey, entity);
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

        [BurstCompile]
        private void CreateCollisionEvent(int sortKey, Entity source, Entity target, float3 impactPosition, in DynamicBuffer<PayloadRequest> payloadBuffer)
        {
            Entity collisionEvent = ECB.CreateEntity(sortKey);
            ECB.AddComponent(
                sortKey,
                collisionEvent,
                new CollisionEvent
                {
                    ProjectileEntity = source,
                    TargetEntity = target,
                    ImpactPosition = impactPosition,
                }
            );

            DynamicBuffer<PayloadRequest> eventPayloadBuffer = ECB.AddBuffer<PayloadRequest>(sortKey, collisionEvent);
            for (int j = 0; j < payloadBuffer.Length; j++)
            {
                eventPayloadBuffer.Add(payloadBuffer[j]);
            }
        }
    }
}
