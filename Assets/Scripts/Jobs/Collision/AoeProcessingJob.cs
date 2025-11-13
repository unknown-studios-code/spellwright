using Spellwright.Components.Collision;
using Spellwright.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Spellwright.Jobs.Collision
{
    [BurstCompile]
    public partial struct AoeProcessingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public CollisionWorld CollisionWorld;

        [ReadOnly]
        public ComponentLookup<DamageableTag> DamageableLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in AoeRequest request)
        {
            var hits = new NativeList<DistanceHit>(Allocator.Temp);
            var filter = CollisionUtils.CreateEnvironmentFilter();

            if (CollisionWorld.OverlapSphere(request.Position, request.Radius, ref hits, filter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    DistanceHit hit = hits[i];

                    if (!DamageableLookup.HasComponent(hit.Entity))
                    {
                        continue;
                    }

                    if (hit.Entity == request.SourceEntity)
                    {
                        continue;
                    }

                    if (request.CheckLineOfSight)
                    {
                        if (!CollisionUtils.HasLineOfSight(CollisionWorld, request.Position, hit.Entity, TransformLookup))
                        {
                            continue;
                        }
                    }

                    Entity collisionEvent = ECB.CreateEntity(sortKey);
                    ECB.AddComponent(
                        sortKey,
                        collisionEvent,
                        new Components.Collision.CollisionEvent
                        {
                            ProjectileEntity = request.SourceEntity,
                            TargetEntity = hit.Entity,
                            ImpactPosition = hit.Position,
                        }
                    );
                }
            }

            hits.Dispose();
            ECB.DestroyEntity(sortKey, entity);
        }
    }
}
