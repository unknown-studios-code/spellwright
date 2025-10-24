using Unity.Entities;

namespace Spellwright.Components.Generators
{
    /// <summary>
    /// Tag component identifying this entity as a projectile-type spell.
    /// Projectiles move in straight line at configured speed, the most common spell delivery type.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Generator (Mandatory - exactly one generator per spell)
    /// Compatible Modifiers: Homing, Pierce, Chain, Multi-Shot, Fork
    /// Compatible Payloads: All payload types (Damage, Heal, Status Effects)
    /// Systems: Used by ProjectileGeneratorSystem, CollisionDetectionSystem, ProjectileMovementSystem
    /// Performance: Zero-size tag component, no memory overhead, enables efficient ECS queries
    /// Query Pattern: <c>WithAll&lt;ProjectileTag&gt;()</c> or <c>WithAll&lt;ProjectileTag, Velocity&gt;()</c>
    /// JSON Mapping: Maps to <c>generator.type = "projectile"</c> in spell JSON schema
    /// Usage: ~80% of spells in typical action games use projectile delivery
    /// </remarks>
    public struct ProjectileTag : IComponentData { }
}

