using Spellwright.Components.Collision;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Spellwright.Utilities
{
    public static class CollisionUtils
    {
        public static CollisionFilter CreateEnvironmentFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = CollisionLayers.ENVIRONMENT,
                CollidesWith = CollisionLayers.ENEMY | CollisionLayers.PLAYER,
                GroupIndex = 0,
            };
        }

        public static CollisionFilter CreateLineOfSightFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = CollisionLayers.ENVIRONMENT | CollisionLayers.ENEMY,
                GroupIndex = 0,
            };
        }

        public static bool IsInCone(in float3 coneOrigin, in float3 coneForward, float cosHalfAngle, in float3 targetPosition)
        {
            float3 toTarget = targetPosition - coneOrigin;
            float distanceSquared = math.lengthsq(toTarget);

            if (distanceSquared < CollisionConstants.EPSILON)
            {
                return true;
            }

            float3 directionToTarget = math.normalize(toTarget);
            float dotProduct = math.dot(coneForward, directionToTarget);

            return dotProduct >= cosHalfAngle;
        }

        public static bool HasLineOfSight(in CollisionWorld collisionWorld, in float3 origin, Entity targetEntity, in ComponentLookup<LocalTransform> transformLookup)
        {
            if (!transformLookup.HasComponent(targetEntity))
            {
                return false;
            }

            float3 targetPosition = transformLookup[targetEntity].Position;
            float3 direction = targetPosition - origin;
            float distance = math.length(direction);

            if (distance < CollisionConstants.MIN_RAYCAST_DISTANCE)
            {
                return true;
            }

            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = targetPosition,
                Filter = CreateLineOfSightFilter(),
            };

            return !collisionWorld.CastRay(raycastInput, out RaycastHit rayHit) || rayHit.Entity == targetEntity;
        }

        public static bool HasLineOfSight(in CollisionWorld collisionWorld, in float3 origin, in float3 targetPosition)
        {
            float3 direction = targetPosition - origin;
            float distance = math.length(direction);

            if (distance < CollisionConstants.MIN_RAYCAST_DISTANCE)
            {
                return true;
            }

            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = targetPosition,
                Filter = CreateLineOfSightFilter(),
            };

            return !collisionWorld.CastRay(raycastInput, out _);
        }

        public static bool IsPointTooClose(in float3 pointA, in float3 pointB, float threshold = CollisionConstants.EPSILON)
        {
            float distanceSquared = math.distancesq(pointA, pointB);
            return distanceSquared < threshold * threshold;
        }
    }
}
