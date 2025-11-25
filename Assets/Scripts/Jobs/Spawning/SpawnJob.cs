using Spellwright.Components.Common;
using Spellwright.Components.Spawning;
using Spellwright.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Spellwright.Jobs.Spawning
{
    [BurstCompile]
    public partial struct SpawnJob : IJobEntity
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

        [ReadOnly]
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

        public double CurrentTime;

        private void Execute([EntityIndexInQuery] int sortKey, Entity entity, in SpawnRequest request)
        {
            Entity spell = ECB.Instantiate(sortKey, request.PrefabEntity);

            float scale = TransformLookup[request.PrefabEntity].Scale;
            float speed = SpeedLookup[request.PrefabEntity].Value;
            float duration = LifetimeLookup[request.PrefabEntity].Duration;

            SetupOwner(sortKey, spell, request.CasterEntity);
            SetupTransform(sortKey, spell, request.SpawnPosition, request.SpawnDirection, scale);
            SetupPhysicsVelocity(sortKey, spell, request.SpawnDirection, speed);
            SetupLifetime(sortKey, spell, duration);

            ECB.DestroyEntity(sortKey, entity);
        }

        private void SetupOwner(int sortKey, Entity spell, Entity caster)
        {
            ECB.SetComponent(sortKey, spell, new SpellOwner { OwnerEntity = caster });
        }

        private void SetupTransform(int sortKey, Entity spell, float3 position, float3 direction, float scale)
        {
            float3 safeDirection = MathUtils.GetSafeDirection(direction);
            var rotation = quaternion.LookRotation(safeDirection, math.up());

            ECB.SetComponent(
                sortKey,
                spell,
                new LocalTransform
                {
                    Position = position,
                    Rotation = rotation,
                    Scale = scale,
                }
            );
        }

        private void SetupPhysicsVelocity(int sortKey, Entity spell, float3 direction, float speed)
        {
            float3 safeDirection = MathUtils.GetSafeDirection(direction);
            float3 linearVelocity = safeDirection * speed;

            ECB.SetComponent(sortKey, spell, new PhysicsVelocity { Linear = linearVelocity, Angular = float3.zero });
        }

        private void SetupLifetime(int sortKey, Entity spell, float duration)
        {
            ECB.SetComponent(sortKey, spell, new Lifetime { SpawnTime = CurrentTime, Duration = duration });
        }
    }
}
