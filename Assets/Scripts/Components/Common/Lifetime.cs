using Unity.Entities;

namespace Spellwright.Components.Common
{
    public struct Lifetime : IComponentData
    {
        public double SpawnTime;
        public float Duration;
    }
}
