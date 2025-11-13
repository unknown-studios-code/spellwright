using Unity.Entities;

namespace Spellwright.Components.Generators
{
    public struct ConeGeneratorData : IComponentData
    {
        public float Radius;
        public float Angle;
    }
}
