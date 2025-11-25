using System;
using Spellwright.Components.LLM;
using Unity.Collections;
using Unity.Entities;

namespace Spellwright.Systems.LLM
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class LLMSystem : SystemBase
    {
        private EntityQuery _referenceQuery;
        private EntityQuery _requestQuery;

        protected override void OnCreate()
        {
            _referenceQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LLMReferenceComponent>().Build(this);
            _requestQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LLMRequestComponent>().WithNone<LLMProcessingTag, LLMResponseComponent>().Build(this);

            RequireForUpdate(_referenceQuery);
            RequireForUpdate(_requestQuery);
        }

        protected override void OnUpdate()
        {
            Entity referenceEntity = _referenceQuery.GetSingletonEntity();
            LLMReferenceComponent llmRef = EntityManager.GetComponentObject<LLMReferenceComponent>(referenceEntity);

            using NativeArray<Entity> entities = _requestQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                LLMRequestComponent request = EntityManager.GetComponentObject<LLMRequestComponent>(entity);

                EntityManager.AddComponent<LLMProcessingTag>(entity);
                var response = new LLMResponseComponent();
                EntityManager.AddComponentObject(entity, response);

                SendChatRequestAsync(llmRef.Value, request.Prompt, entity, response);
            }
        }

        private async void SendChatRequestAsync(LLMUnity.LLMCharacter llm, string prompt, Entity entity, LLMResponseComponent response)
        {
            try
            {
                string result = await llm.Chat(
                    prompt,
                    (content) =>
                    {
                        if (EntityManager.Exists(entity))
                        {
                            response.Content = content;
                        }
                    },
                    () =>
                    {
                        if (EntityManager.Exists(entity))
                        {
                            response.IsComplete = true;
                            EntityManager.RemoveComponent<LLMProcessingTag>(entity);
                        }
                    },
                    addToHistory: true
                );

                if (result == null && EntityManager.Exists(entity) && !response.IsComplete)
                {
                    HandleError(entity, response, "LLM returned null response.");
                }
            }
            catch (Exception e)
            {
                HandleError(entity, response, e.Message);
            }
        }

        private void HandleError(Entity entity, LLMResponseComponent response, string message)
        {
            if (!EntityManager.Exists(entity))
            {
                return;
            }

            response.IsError = true;
            response.ErrorMessage = message;

            if (EntityManager.HasComponent<LLMProcessingTag>(entity))
            {
                EntityManager.RemoveComponent<LLMProcessingTag>(entity);
            }
        }
    }
}
