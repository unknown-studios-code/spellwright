using Unity.Entities;

namespace Spellwright.Components.Modifiers
{
    public struct HomingModifier : IComponentData
    {
        public float TurnSpeed;
        public Entity Target;
    }
}
