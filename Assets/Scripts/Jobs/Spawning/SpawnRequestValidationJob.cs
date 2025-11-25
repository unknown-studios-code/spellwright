using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Spellwright.Jobs.Spawning
{
    [BurstCompile]
    public partial struct SpawnRequestValidationJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [ReadOnly]
        public ComponentLookup<SpellOwner> SpellOwnerLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        [ReadOnly]
        public ComponentLookup<Speed> SpeedLookup;

        [ReadOnly]
        public ComponentLookup<Lifetime> LifetimeLookup;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in SpawnRequest request)
        {
            if (IsValidPrefab(request.PrefabEntity, entity))
            {
                ECB.AddComponent<ValidatedTag>(sortKey, entity);
            }
            else
            {
                ECB.DestroyEntity(sortKey, entity);
            }
        }

        [BurstCompile]
        private bool IsValidPrefab(Entity prefabEntity, Entity requestEntity)
        {
            bool hasSpellOwner = SpellOwnerLookup.HasComponent(prefabEntity);
            bool hasTransform = TransformLookup.HasComponent(prefabEntity);
            bool hasSpeed = SpeedLookup.HasComponent(prefabEntity);
            bool hasLifetime = LifetimeLookup.HasComponent(prefabEntity);

            bool isValid = hasSpellOwner && hasTransform && hasSpeed && hasLifetime;

            LogValidationErrors(prefabEntity, requestEntity, hasSpellOwner, hasTransform, hasSpeed, hasLifetime);

            return isValid;
        }

        [BurstDiscard]
        private static void LogValidationErrors(Entity prefabEntity, Entity requestEntity, bool hasSpellOwner, bool hasTransform, bool hasSpeed, bool hasLifetime)
        {
            if (!hasSpellOwner)
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing SpellOwner component"
                );
            }

            if (!hasTransform)
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing LocalTransform component"
                );
            }

            if (!hasSpeed)
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing Speed component"
                );
            }

            if (!hasLifetime)
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing Lifetime component"
                );
            }
        }
    }
}
