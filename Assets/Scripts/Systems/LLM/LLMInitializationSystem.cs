using LLMUnity;
using Spellwright.Components.LLM;
using Spellwright.Utilities;
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
            if (llmCharacter == null)
            {
                Debug.LogError($"[LLMInitializationSystem] LLMCharacter not found. LLM system will not run.");
                Enabled = false;
                return;
            }

            TextAsset schemaAsset = Resources.Load<TextAsset>(LLMConstants.SCHEMA_RESOURCE_PATH);
            if (schemaAsset == null)
            {
                Debug.LogError($"[LLMInitializationSystem] SpellSchema not found at Resources/{LLMConstants.SCHEMA_RESOURCE_PATH}. LLM system will not run.");
                Enabled = false;
                return;
            }

            Entity entity = EntityManager.CreateEntity();
            EntityManager.AddComponentObject(entity, new LLMReferenceComponent { LLMCharacter = llmCharacter, JsonSchema = schemaAsset.text });

            Enabled = false;
        }
    }
}
