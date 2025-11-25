using Unity.Entities;

namespace Spellwright.Components.LLM
{
    public class LLMResponseComponent : IComponentData
    {
        public string Content;
        public bool IsComplete;
        public bool IsError;
        public string ErrorMessage;
    }
}
