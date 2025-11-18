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
            bool isValid = true;

            if (!SpellOwnerLookup.HasComponent(prefabEntity))
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing SpellOwner component"
                );
                isValid = false;
            }

            if (!TransformLookup.HasComponent(prefabEntity))
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing LocalTransform component"
                );
                isValid = false;
            }

            if (!SpeedLookup.HasComponent(prefabEntity))
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing Speed component"
                );
                isValid = false;
            }

            if (!LifetimeLookup.HasComponent(prefabEntity))
            {
                UnityEngine.Debug.LogWarning(
                    $"[SpawnRequestValidation] SpawnRequest {requestEntity.Index}:{requestEntity.Version} rejected: Prefab {prefabEntity.Index}:{prefabEntity.Version} missing Lifetime component"
                );
                isValid = false;
            }

            return isValid;
        }
    }
}
