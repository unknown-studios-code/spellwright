using Spellwright.Components.StatusEffect;
using Spellwright.Jobs.StatusEffect;
using Unity.Burst;
using Unity.Entities;

namespace Spellwright.Systems.StatusEffect
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Health.HealthModificationSystem))]
    public partial struct StatusEffectProcessingSystem : ISystem
    {
        private EntityQuery _statusEffectQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _statusEffectQuery = SystemAPI.QueryBuilder().WithAll<StatusEffectStack>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_statusEffectQuery.IsEmpty)
                return;

            float deltaTime = SystemAPI.Time.DeltaTime;
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ProcessStatusEffectsJob { DeltaTime = deltaTime, ECB = ecb.AsParallelWriter() };

            state.Dependency = job.ScheduleParallel(_statusEffectQuery, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}
