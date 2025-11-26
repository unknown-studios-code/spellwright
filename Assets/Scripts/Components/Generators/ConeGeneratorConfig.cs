using Unity.Entities;

namespace Spellwright.Components.Generators
{
    public struct ConeGeneratorConfig : IComponentData
    {
        public float Radius;
        public float Angle;
    }
}
