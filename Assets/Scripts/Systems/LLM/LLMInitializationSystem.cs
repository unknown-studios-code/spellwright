using LLMUnity;
using Spellwright.Components.LLM;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Spellwright.Systems.LLM
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class LLMInitializationSystem : SystemBase
    {
        private EntityQuery _singletonQuery;

        protected override void OnCreate()
        {
            _singletonQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LLMReferenceComponent>().Build(this);
        }

        protected override void OnUpdate()
        {
            if (!_singletonQuery.IsEmptyIgnoreFilter)
            {
                Enabled = false;
                return;
            }

            LLMCharacter llmCharacter = Object.FindFirstObjectByType<LLMCharacter>();
            if (llmCharacter != null)
            {
                Entity entity = EntityManager.CreateEntity();
                EntityManager.AddComponentObject(entity, new LLMReferenceComponent { Value = llmCharacter });
            }
        }
    }
}
