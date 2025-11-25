using LLMUnity;
using Unity.Entities;

namespace Spellwright.Components.LLM
{
    public class LLMReferenceComponent : IComponentData
    {
        public LLMCharacter LLMCharacter;
        public string JsonSchema;
    }
}
