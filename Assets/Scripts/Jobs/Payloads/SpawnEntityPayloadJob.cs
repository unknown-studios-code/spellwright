using Spellwright.Components.Collision;
using Spellwright.Components.Payloads;
using Spellwright.Components.Spawning;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Jobs.Payloads
{
    [BurstCompile]
    public partial struct SpawnEntityPayloadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int sortKey, in DynamicBuffer<CollisionHit> collisions, in DynamicBuffer<SpawnEntityRequest> payloads)
        {
            foreach (CollisionHit collision in collisions)
            {
                foreach (SpawnEntityRequest payload in payloads)
                {
                    if (payload.Prefab == Entity.Null)
                    {
                        continue;
                    }

                    Entity spawnedEntity = ECB.Instantiate(sortKey, payload.Prefab);
                    ECB.AddComponent(
                        sortKey,
                        spawnedEntity,
                        new SpawnRequest
                        {
                            PrefabEntity = payload.Prefab,
                            CasterEntity = Entity.Null,
                            SpawnPosition = collision.ImpactPosition,
                            SpawnDirection = default,
                        }
                    );
                }
            }
        }
    }
}
