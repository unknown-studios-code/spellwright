using Unity.Entities;

namespace Spellwright.Components.Generators
{
    /// <summary>
    /// Tag component identifying this entity as an area-of-effect (AoE) spell.
    /// AoE spells affect all targets uniformly within a circular radius, regardless of direction.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Generator (Mandatory - exactly one generator per spell)
    /// Compatible Modifiers: Pierce, Chain (Homing not applicable - AoE is stationary)
    /// Compatible Payloads: All payload types (Damage, Heal, Status Effects)
    /// Systems: Used by AoEGeneratorSystem, CollisionDetectionSystem
    /// Performance: Zero-size tag component, no memory overhead, enables efficient ECS queries
    /// Query Pattern: <c>WithAll&lt;AreaOfEffectTag&gt;()</c> or <c>WithAll&lt;AreaOfEffectTag, GeneratorData&gt;()</c>
    /// JSON Mapping: Maps to <c>generator.type = "area_of_effect"</c> in spell JSON schema
    /// Geometry: Uses simple distance check for circular area detection (most efficient)
    /// Modes: Supports pulsing (repeated application) and one-shot (single application)
    /// Examples: Fire Nova, Healing Circle, Poison Cloud, Ground Slam
    /// </remarks>
    public struct AreaOfEffectTag : IComponentData { }
}

