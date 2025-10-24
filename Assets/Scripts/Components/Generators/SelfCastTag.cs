using Unity.Entities;

namespace Spellwright.Components.Generators
{
    /// <summary>
    /// Tag component identifying this entity as a self-cast spell.
    /// Self-cast spells affect only the caster, requiring no collision detection or spatial queries.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Generator (Mandatory - exactly one generator per spell)
    /// Compatible Modifiers: None (self-cast is instant and targets only caster)
    /// Compatible Payloads: All payload types (Damage, Heal, Status Effects)
    /// Systems: Used by SelfCastGeneratorSystem, PayloadResolutionSystem
    /// Performance: Zero-size tag component, no memory overhead, most efficient generator type
    /// Query Pattern: <c>WithAll&lt;SelfCastTag&gt;()</c> or <c>WithAll&lt;SelfCastTag, SpellOwner&gt;()</c>
    /// JSON Mapping: Maps to <c>generator.type = "self_cast"</c> in spell JSON schema
    /// Execution: Instant application at spawn time, no movement or collision required
    /// Examples: Heal Self, Divine Shield, Berserk Rage, Self-buff abilities
    /// Use Case: Essential for buffs, self-heals, and instant abilities
    /// </remarks>
    public struct SelfCastTag : IComponentData { }
}

