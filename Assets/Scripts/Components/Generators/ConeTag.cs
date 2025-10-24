using Unity.Entities;

namespace Spellwright.Components.Generators
{
    /// <summary>
    /// Tag component identifying this entity as a cone-type spell.
    /// Cones affect targets within a directional sector defined by angle and radius.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Generator (Mandatory - exactly one generator per spell)
    /// Compatible Modifiers: Pierce, Chain, Multi-Shot (Homing not applicable - cone is stationary)
    /// Compatible Payloads: All payload types (Damage, Heal, Status Effects)
    /// Systems: Used by ConeGeneratorSystem, CollisionDetectionSystem
    /// Performance: Zero-size tag component, no memory overhead, enables efficient ECS queries
    /// Query Pattern: <c>WithAll&lt;ConeTag&gt;()</c> or <c>WithAll&lt;ConeTag, GeneratorData&gt;()</c>
    /// JSON Mapping: Maps to <c>generator.type = "cone"</c> in spell JSON schema
    /// Geometry: Uses dot product for angular range calculation within cone sector
    /// Examples: Flame Breath, Frost Cone, Cleave attacks
    /// </remarks>
    public struct ConeTag : IComponentData { }
}

