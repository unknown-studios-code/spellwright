using System;
using System.Threading.Tasks;
using LLMUnity;
using Spellwright.Components.LLM;
using Spellwright.Utilities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Spellwright.Systems.LLM
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class LLMSystem : SystemBase
    {
        private EntityQuery _referenceQuery;
        private EntityQuery _requestQuery;
        private LLMCharacter _llmCharacter;

        protected override void OnCreate()
        {
            _referenceQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LLMReferenceComponent>().Build(this);
            _requestQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LLMRequestComponent>().WithNone<LLMProcessingTag, LLMResponseComponent>().Build(this);

            RequireForUpdate(_referenceQuery);
            RequireForUpdate(_requestQuery);
        }

        protected override void OnStartRunning()
        {
            Entity referenceEntity = _referenceQuery.GetSingletonEntity();
            LLMReferenceComponent llmRef = EntityManager.GetComponentObject<LLMReferenceComponent>(referenceEntity);

            _llmCharacter = llmRef.LLMCharacter;
            _llmCharacter.grammarJSONString = llmRef.JsonSchema;
            _llmCharacter.SetPrompt(LLMConstants.SYSTEM_PROMPT, clearChat: true);
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = _requestQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                LLMRequestComponent request = EntityManager.GetComponentObject<LLMRequestComponent>(entity);

                Debug.Log($"[LLMSystem] Processing request with System Prompt:\n{_llmCharacter.prompt}");

                EntityManager.AddComponent<LLMProcessingTag>(entity);
                var response = new LLMResponseComponent();
                EntityManager.AddComponentObject(entity, response);

                SendChatRequestAsync(request.Prompt, entity, response);
            }
        }

        private async void SendChatRequestAsync(string userInput, Entity entity, LLMResponseComponent response)
        {
            int attempt = 0;

            while (attempt < LLMConstants.MAX_RETRIES)
            {
                attempt++;

                if (!EntityManager.Exists(entity))
                {
                    return;
                }

                try
                {
                    Debug.Log($"[LLMSystem] Attempt {attempt}/{LLMConstants.MAX_RETRIES}: \"{userInput}\"");

                    string result = await _llmCharacter.Chat(
                        userInput,
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
                        addToHistory: false
                    );

                    if (result != null)
                    {
                        Debug.Log($"[LLMSystem] Success on attempt {attempt}");
                        return;
                    }

                    if (attempt < LLMConstants.MAX_RETRIES)
                    {
                        await WaitForRetryAsync(attempt, "Null response");
                    }
                }
                catch (Exception e)
                {
                    if (attempt >= LLMConstants.MAX_RETRIES)
                    {
                        HandleError(entity, response, $"Failed after {attempt} attempts: {e.Message}");
                        return;
                    }

                    await WaitForRetryAsync(attempt, $"Error: {e.Message}");
                }
            }

            HandleError(entity, response, $"Max retries ({LLMConstants.MAX_RETRIES}) exceeded with null response");
        }

        private async Task WaitForRetryAsync(int attempt, string reason)
        {
            float jitter = UnityEngine.Random.Range(0f, LLMConstants.JITTER_MAX_SECONDS);
            float delay = LLMConstants.BASE_DELAY_SECONDS + (LLMConstants.ADDITIONAL_DELAY_SECONDS * (attempt - 1)) + jitter;
            Debug.LogWarning($"[LLMSystem] {reason}, retrying after {delay * 1000:F0}ms...");
            await Task.Delay(TimeSpan.FromSeconds(delay));
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
