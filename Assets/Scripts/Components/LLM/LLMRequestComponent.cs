using Unity.Entities;

namespace Spellwright.Components.LLM
{
    public class LLMRequestComponent : IComponentData
    {
        public string Prompt;
    }
}
