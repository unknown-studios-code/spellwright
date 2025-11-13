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
    public partial struct AoeProcessingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public CollisionWorld CollisionWorld;

        [ReadOnly]
        public ComponentLookup<HealthComponent> HealthLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in AoeRequest request, in DynamicBuffer<PayloadRequest> payloadBuffer)
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);
            CollisionFilter filter = CollisionUtils.CreateEnvironmentFilter();

            if (CollisionWorld.OverlapSphere(request.Position, request.Radius, ref hits, filter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    DistanceHit hit = hits[i];

                    if (!IsValidTarget(hit.Entity, request.SourceEntity))
                        continue;
                    if (!HasLineOfSight(hit.Entity, request.Position, request.CheckLineOfSight))
                        continue;

                    CreateCollisionEvent(sortKey, request.SourceEntity, hit.Entity, hit.Position, payloadBuffer);
                }
            }

            hits.Dispose();
            ECB.DestroyEntity(sortKey, entity);
        }

        [BurstCompile]
        private bool IsValidTarget(Entity target, Entity sourceEntity)
        {
            if (!HealthLookup.HasComponent(target))
                return false;
            if (target == sourceEntity)
                return false;
            return true;
        }

        [BurstCompile]
        private bool HasLineOfSight(Entity target, float3 origin, bool checkLineOfSight)
        {
            if (!checkLineOfSight)
                return true;
            return CollisionUtils.HasLineOfSight(CollisionWorld, origin, target, TransformLookup);
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
