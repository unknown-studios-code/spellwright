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
    public partial struct DetectAoeCollisionJob : IJobEntity
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
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in AoeRequest request)
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);

            if (CollisionWorld.OverlapSphere(request.Position, request.Radius, ref hits, EnvironmentFilter))
            {
                DynamicBuffer<CollisionHit> buffer = ECB.AddBuffer<CollisionHit>(sortKey, entity);

                for (int i = 0; i < hits.Length; i++)
                {
                    DistanceHit hit = hits[i];

                    if (!IsValidTarget(hit.Entity, request.SourceEntity))
                    {
                        continue;
                    }

                    if (!HasLineOfSight(hit.Entity, request.Position, request.CheckLineOfSight))
                    {
                        continue;
                    }

                    buffer.Add(new CollisionHit { TargetEntity = hit.Entity, ImpactPosition = hit.Position });
                }
            }

            hits.Dispose();
            ECB.RemoveComponent<AoeRequest>(sortKey, entity);
        }

        [BurstCompile]
        private bool IsValidTarget(Entity target, Entity sourceEntity)
        {
            return HealthLookup.HasComponent(target) && target != sourceEntity;
        }

        [BurstCompile]
        private readonly bool HasLineOfSight(Entity target, float3 origin, bool checkLineOfSight)
        {
            return !checkLineOfSight || CollisionUtils.HasLineOfSight(CollisionWorld, origin, target, TransformLookup);
        }
    }
}
