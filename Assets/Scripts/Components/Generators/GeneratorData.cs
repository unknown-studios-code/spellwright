using Unity.Entities;

namespace Spellwright.Components.Generators
{
    public struct GeneratorData : IComponentData
    {
        public float Speed;
        public float Radius;
        public float Angle;
        public float Lifetime;
    }
}
