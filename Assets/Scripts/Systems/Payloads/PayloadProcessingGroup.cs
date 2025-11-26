using Unity.Entities;

namespace Spellwright.Systems.Payloads
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PayloadProcessingGroup : ComponentSystemGroup { }
}
