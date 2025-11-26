using Spellwright.Components.StatusEffect;
using Spellwright.Jobs.StatusEffect;
using Spellwright.Systems.Health;
using Spellwright.Systems.Payloads;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.StatusEffect
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PayloadProcessingGroup))]
    [UpdateBefore(typeof(HealthUpdateSystem))]
    public partial struct StatusEffectSystem : ISystem
    {
        private EntityQuery _statusEffectQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _statusEffectQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<StatusEffectStack>().Build(ref state);

            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(_statusEffectQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ApplyStatusEffectsJob { DeltaTime = deltaTime, ECB = ecb.AsParallelWriter() };

            state.Dependency = job.ScheduleParallel(_statusEffectQuery, state.Dependency);
        }
    }
}
