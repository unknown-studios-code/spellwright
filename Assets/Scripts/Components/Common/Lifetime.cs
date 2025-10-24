using Unity.Entities;

namespace Spellwright.Components
{
    public struct Lifetime : IComponentData
    {
        public double SpawnTime;
        public float Duration;
    }
}
